﻿using Memory;
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
using System.Numerics;
using System.Globalization;
namespace AssaultCubeTrainer
{
    class TrainerMain
    {
        public Boolean isAttached = false;
        public Boolean EspEnabled = false;
        public Boolean AimbotEnabled = false;
        public Boolean KeepAttributes = false;
        public int lifeValue = 100;
        public int ammo1Value = 100;
        public int grenadesValue = 100;
        public Mem m = new Mem();
        public Process gameProcess;
        public string gameProcessId;
        public Thread threadHacking;
        public Func<Entity, Size, Boolean> UpdateStates;

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
                            List<Entity> allPlayers = loadEntities();

                            List<Entity> enemies = allPlayers.Where(p => (p.team >= 0 && p.team <=100 && p.hp > 0 && p.hp <= 100 && p.team != localPlayer.team)).ToList();

                            Size gameProcessWinSize = Drawing.GetSize(gameProcessName);

                            if (localPlayer != null) {
                                UpdateStates(localPlayer, gameProcessWinSize);
                            }

                            StartKeepAttributes();
                            StartEsp(gameProcessWinSize, localPlayer, enemies);
                            StartAimbot(gameProcessWinSize, localPlayer, enemies);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Missed the win handler")
                        {
                            isAttached = false;
                            attachGame(gameProcessName);
                        }
                        else
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            });

            threadHacking.IsBackground = true;
            threadHacking.Start();

            return;
        }
        public void StartAimbot(Size gameProcessWinSize, Entity player, List<Entity> enemies)
        {
            if (AimbotEnabled)
            {
                try
                {
                    if (enemies.Count > 0)
                    {
                       string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                       
                        string viewXAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleX);
                        string viewYAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleY);
                        float currYaw = m.ReadFloat(viewXAddress); // de 0 a 360
                        float currPitch = m.ReadFloat(viewYAddress); // de -90 a 90

                        List<Vector2> ClosestPitchesAndYaws = new List<Vector2>();

                        foreach (Entity enemy in enemies)
                        {
                            if (enemy.hp < 0) continue;
                            
                            ViewMatrix viewMatrix = ESP.GetViewMatrix(m);

                            PointF screenPos = ESP.WorldToScreen(viewMatrix, enemy, gameProcessWinSize);

                            // Verificar se a posição está dentro da tela
                            if (screenPos.X >= 0 && screenPos.X <= gameProcessWinSize.Width && screenPos.Y >= 0 && screenPos.Y <= gameProcessWinSize.Height && player.hp > 0) {
                            
                                Aimbot.CalculateAim(player, enemy, out float targetPitch, out float targetYaw);
                                ClosestPitchesAndYaws.Add(new Vector2{ x=targetYaw, y=targetPitch,  } );
                            }
                        }

                        if (ClosestPitchesAndYaws.Count > 0)
                        {
                            Vector2 closestTarget = ClosestPitchesAndYaws[0];
                            float smallestDistance = float.MaxValue;

                            foreach (var aim in ClosestPitchesAndYaws)
                            {
                                float yawDiff = Math.Abs(currYaw - aim.x);
                                float pitchDiff = Math.Abs(currPitch - aim.y);
                                float totalDiff = yawDiff + pitchDiff;

                                if (totalDiff < smallestDistance)
                                {
                                    smallestDistance = totalDiff;
                                    closestTarget = aim;
                                }
                            }

                            m.WriteMemory(viewXAddress, "float", closestTarget.x.ToString(CultureInfo.InvariantCulture));
                            m.WriteMemory(viewYAddress, "float", closestTarget.y.ToString(CultureInfo.InvariantCulture));
                        }

                    }
                    else
                    {
                        Console.WriteLine("Nenhum inimigo encontrado.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao iniciar o aimbot: {e.Message}");
                }
            }
        }
        public void StartEsp(Size gameProcessWinSize, Entity localPlayer, List<Entity> enemies)
        {
            if (EspEnabled)
            {                    
                    ViewMatrix viewMatrix = ESP.GetViewMatrix(m);
                    ESP.ShowESP(viewMatrix, enemies, localPlayer, gameProcessWinSize, gameProcess);
            }
        }

        public void StartKeepAttributes()
        {
            if (KeepAttributes && isAttached)
            {
                TrySetAttributes(lifeValue.ToString(), ammo1Value.ToString(), grenadesValue.ToString());
            }
        }

        public Entity getLocalPlayer()
        {
            string localPlayerHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");

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
            string entityList = m.ReadInt(Offsets.EntityList).ToString("x");
            int maxPlayers = 32;

            entities.Clear();
            for (int i = 4; i <= 4 * maxPlayers; i += 4)
            {
                string playerEntityHex = m.ReadInt(Utils.SumOfHexStrings(entityList, i.ToString("x"))).ToString("x");

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
                gameProcessId = null;
                return null;
            }
            Console.WriteLine(procIdGame);
            gameProcess = Process.GetProcessById(procIdGame);
            if (gameProcess == null)
            {
                isAttached = false;
                return null;
            }
            m.OpenProcess(procIdGame);
            isAttached = true;
            gameProcessId = procIdGame.ToString();


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
