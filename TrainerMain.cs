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
        public string gameProcessId;
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

                            StartEsp(gameProcessWinSize, localPlayer, gameProcessName);
                            StartAimbot(gameProcessWinSize, localPlayer);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Thread.Sleep(300);
                }
            });

            threadHacking.IsBackground = true;
            threadHacking.Start();

            return;
        }
        public void StartAimbot(Size gameProcessWinSize, Entity localPlayer)
        {
            if (AimbotEnabled)
            {
                try
                {
                    // Carrega todos os jogadores
                    List<Entity> allPlayers = loadEntities();
                    // Filtra apenas os inimigos (excluindo a equipe do jogador local)
                    List<Entity> enemies = allPlayers.Where(p => p.team != localPlayer.team).ToList();

                    // Verifica se há inimigos
                    if (enemies.Count > 0)
                    {
                        // Variáveis para armazenar o inimigo mais próximo
                        // Entity closestEnemy = null;
                        float closestDistance = float.MaxValue;
                        float closestYaw = 0.0f;
                        float closestPitch = 0.0f;

                        // Itera sobre todos os inimigos
                        foreach (Entity enemy in enemies)
                        {
                            ViewMatrix view = ESP.GetViewMatrix(m);
                            // Obtém os endereços de memória para os ângulos de visão do jogador local
                            string PlayerEntityMemoryHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");
                            string viewXAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleX);
                            string viewYAddress = Utils.SumOfHexStrings(PlayerEntityMemoryHex, Offsets.m_ViewAngleY);

                            // Lê os valores atuais de pitch e yaw do jogador local
                            string currPitch = m.ReadFloat(viewYAddress).ToString();
                            string currYaw = m.ReadFloat(viewXAddress).ToString();

                            // Calcula a distância horizontal do inimigo
                            float abspos_x = enemy.x - localPlayer.x;
                            float abspos_y = enemy.y - localPlayer.y;
                            float abspos_z = enemy.z + 0.3f - localPlayer.z; // Adiciona 0.3 para mirar na cabeça
                            float Horizontal_distance = (float)Math.Sqrt(abspos_x * abspos_x + abspos_y* abspos_y);

                            // Calcula a distância 2D (usada para verificar se o inimigo está dentro da mira)
                            float screenX = (view.m11 * enemy.x) + (view.m21 * enemy.y) + (view.m31 * enemy.z) + view.m41;
                            float screenY = (view.m12 * enemy.x) + (view.m22 * enemy.y) + (view.m32 * enemy.z) + view.m42;
                            float screenW = (view.m14 * enemy.x) + (view.m24 * enemy.y) + (view.m34 * enemy.z) + view.m44;
                            if (screenW <= 0.001f)
                            {
                                continue; // Se a distância for muito pequena, ignore esse inimigo
                            }

                            // Converte a posição do inimigo para o espaço da tela
                            float camX = gameProcessWinSize.Width / 2.0f;
                            float camY = gameProcessWinSize.Height / 2.0f;
                            float deltax = (camX * screenX / screenW);
                            float deltay = (camY * screenY / screenW);
                            float distanceToCrosshair = (float)Math.Sqrt(deltax * deltax + deltay * deltay);

                            // Se o inimigo está mais próximo do centro da tela (mira), calcula o yaw e pitch
                            if (distanceToCrosshair < closestDistance)
                            {
                                closestDistance = distanceToCrosshair;

                                // Calcula o yaw (azimutho no plano XY)
                                float azimuth_xy = (float)Math.Atan2(abspos_y, abspos_x);
                                closestYaw = azimuth_xy * (180.0f / (float) Math.PI) + 90; // Ajuste de 90 graus

                                // Calcula o pitch (azimutho vertical)
                                float azimuth_z = (float) Math.Atan2(abspos_z, Horizontal_distance);
                                closestPitch = azimuth_z * (180.0f / (float) Math.PI);
                            }
                            
                            // Se um inimigo foi encontrado, ajusta a mira
                            if (closestDistance != float.MaxValue)
                            {
                                Console.WriteLine($"Ajustando mira para {closestYaw} (Yaw), {closestPitch} (Pitch)");
                                // Atualiza os ângulos de visão do jogador local
                                m.WriteMemory(viewXAddress, "float", closestYaw.ToString());
                                m.WriteMemory(viewYAddress, "float", closestPitch.ToString());
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("Nenhum inimigo encontrado.");
                    }
                }
                catch (Exception e)
                {
                    // Captura e exibe qualquer erro ocorrido
                    Console.WriteLine($"Erro ao iniciar o aimbot: {e.Message}");
                }
            }
        }
        public void StartEsp(Size gameProcessWinSize, Entity localPlayer, string gameProcessName)
        {
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
        }

        public Entity getLocalPlayer()
        {
            string localPlayerHex = m.ReadInt(Offsets.offsetLocalPlayer).ToString("X");

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
