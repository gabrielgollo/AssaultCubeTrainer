using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AssaultCubeTrainer;
using System.Windows.Forms;
using System.Drawing;
namespace AssaultCubeTrainer
{
    class TrainerMain
    {
        public Boolean isAttached = false;
        public Boolean EspEnabled = false;
        public Boolean AimbotEnabled = false;
        public Mem m = new Mem();
        public Process gameProcess;
        public Thread threadHacking;
        // function that will be imported by another class and will receive the Entity and Size to be show in form
        public Func<Entity, Size, Boolean> UpdateStates;

        // List of entities
        List<Entity> entities = new List<Entity>();

        public void TryStartHacking(string gameProcessName)
        {
            attachGame(gameProcessName);
            threadHacking = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (isAttached)
                        {
                            Entity localPlayer = getLocalPlayer();
                            Size gameProcessWinSize = Drawing.GetSize(gameProcessName);
                        
                            if (localPlayer != null) {
                               UpdateStates(localPlayer, gameProcessWinSize);    
                            }

                            if (AimbotEnabled)
                            {
                                try
                                {
                                    List<Entity> allPlayers = loadEntities();
                                    // should filter team
                                    List<Entity> enemies = allPlayers.Where(p => p.team != localPlayer.team).ToList();

                                    //foreach (Entity enemy in enemies)
                                    //{
                                    Aimbot.CalculateAim(localPlayer, enemies[0], 1.0f, out float targetPitch, out float targetYaw, out bool changeVision);

                                    if (changeVision)
                                    {
                                        string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                                        string viewXAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleX);
                                        string viewYAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleY);
                                        m.WriteMemory(viewXAddress, "float", targetYaw.ToString());
                                        m.WriteMemory(viewYAddress, "float", targetPitch.ToString());
                                    }
                                }
                                catch (Exception e)
                                {

                                    Console.WriteLine(e.Message);
                                }
                                
                            }

                            if (EspEnabled)
                            {
                                try
                                {
                                    List<Entity> allPlayers = loadEntities();
                                    // should filter team
                                    List<Entity> enemies = allPlayers.Where(p => p.team != localPlayer.team).ToList();
                                    ViewMatrix viewMatrix = ESP.GetViewMatrix(m);
                                    ESP.ShowESP(viewMatrix, enemies, gameProcessWinSize, gameProcess);
                                }
                                catch (Exception e)
                                {
                                    if(e.Message == "Missed the win handler")
                                    {
                                        isAttached = false;
                                        attachGame(gameProcessName);
                                    } else 
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Thread.Sleep(10);
                }
            });

            threadHacking.IsBackground = true;
            threadHacking.Start();
        }

        public Entity getLocalPlayer()
        {
            string localPlayerHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
            Console.WriteLine("localPlayerHex: " + localPlayerHex);

            // read localplayer x,y,z and views
            float lpX, lpY, lpZ, lpViewX, lpViewY, lpFOV;
            string name = m.ReadString(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_Name));
            lpX = m.ReadFloat(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_XPos));
            lpY = m.ReadFloat(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_YPos));
            lpZ = m.ReadFloat(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_ZPos));
            lpViewX = m.ReadFloat(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_ViewAngleX)); // yaw angle
            lpViewY = m.ReadFloat(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_ViewAngleY)); // pitch angle
            lpFOV = (float)m.ReadInt(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_FOV));

            int life, ammo1;
            life = m.ReadInt(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_Health));
            ammo1 = m.ReadInt(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_Ammo));
            int lpTeam = m.ReadInt(Utils.SumOfHexStrings(localPlayerHex, Offsets.m_Team));

            return new Entity { name = name, x = lpX, y = lpY, z = lpZ, viewX = lpViewX, viewY = lpViewY, fov = lpFOV, hp = life, ammo1 = ammo1, team=lpTeam };
        }

        public List<Entity> loadEntities()
        {
            // get list entity
            string entityList = m.ReadInt(Offsets.EntityList).ToString("x");
            int maxPlayers = 32;

            entities.Clear();
            for (int i = 4; i <= 4 * maxPlayers; i += 4)
            {
                string playerEntityHex = m.ReadInt(Utils.SumOfHexStrings(entityList, i.ToString("x"))).ToString("x");


                // read positions x, y, z, hp and name dos jogadores
                float x = m.ReadFloat(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_XPos));
                float y = m.ReadFloat(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_YPos));
                float z = m.ReadFloat(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_ZPos));
                int hp = m.ReadInt(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_Health));
                string name = m.ReadString(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_Name));
                int team = m.ReadInt(Utils.SumOfHexStrings(playerEntityHex, Offsets.m_Team));
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                Entity player = new Entity { name = name, hp = hp, x = x, y = y, z = z, team = team };
                entities.Add(player);
            }

            return entities;
        }

        public string attachGame(string gameProcessName)
        {
            int procIdGame = m.GetProcIdFromName(gameProcessName);
            if (procIdGame <= 0)
            {
                isAttached = false;
            }
            else
            {
                Console.WriteLine(procIdGame);
                gameProcess = Process.GetProcessById(procIdGame);
                if (gameProcess == null)
                {
                    isAttached = false;
                    return null;
                }
                m.OpenProcess(procIdGame);
                isAttached = true;

            }

            return procIdGame.ToString();
        }

        public Boolean TrySetAttributes(string lifeValue, string ammo1Value, string grenadesValue)
        {
            Boolean hasSettedLife = TrySetLife(lifeValue);
            Boolean hasSettedAmmo = TrySetAmmo1Value(ammo1Value);
            Boolean hasSettedGrenades = TrySetGrenadesValue(grenadesValue);

            return hasSettedLife && hasSettedAmmo && hasSettedGrenades;
        }

        public Boolean TrySetLife(string lifeValue)
        {
            try
            {
                if (!isAttached) return false;
                string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                string lifeAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_Health);
                m.WriteMemory(lifeAddress, "int", lifeValue);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Boolean TrySetAmmo1Value(string ammo1Value)
        {
            try
            {
                if (!isAttached) return false;
                string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                string ammoAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_Ammo);
                m.WriteMemory(ammoAddress, "int", ammo1Value);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Boolean TrySetGrenadesValue(string grenadesValue)
        {
            try
            {
                if (!isAttached) return false;
                string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                string grenadesAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_Grenades);
                m.WriteMemory(grenadesAddress, "int", grenadesValue);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
    }
