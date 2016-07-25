using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csgoDL
{
    /// <summary>
    /// Defines the different available gametypes
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// The arms race / gun game gametype
        /// </summary>
        ArmsRace,

        /// <summary>
        /// The demolition gametype
        /// </summary>
        Demolition,

        /// <summary>
        /// The casual mode of classic gameplay
        /// </summary>
        ClassicCasual,

        /// <summary>
        /// The classical competitive gametype
        /// </summary>
        ClassicCompetitive,

        /// <summary>
        /// Classic deathmatch playstyle
        /// </summary>
        Deathmatch
    }

    /// <summary>
    /// Handles the terminal call.
    /// </summary>
    public class SRCDSWrapper
    {
        #region Constants

        /// <summary>
        /// The filename of the dedicated server.
        /// </summary>
        public const string DEFAULT_SRCDS_EXE_FILENAME = "srcds.exe";
        /// <summary>
        /// The default arguments when executing the dedicated server.s
        /// </summary>
        public const string DEFAULT_ARGUMENTS = "-game csgo -console -usercon";
        /// <summary>
        /// The default argument tail when executing the dedicated server.
        /// </summary>
        public const string DEFAULT_ARGUMENTS_TAIL = " +ip 0.0.0.0 +net_public_adr 0.0.0.0";
        /// <summary>
        /// The arguments to pass in order to start a certain game type.
        /// </summary>
        public static Dictionary<GameType, string> GameTypeArguments = new Dictionary<GameType, string>()
        {
            { GameType.ArmsRace, "+game_type 1 +game_mode 0" },
            { GameType.Demolition, "+game_type 1 +game_mode 1" },
            { GameType.ClassicCasual, "+game_type 0 +game_mode 0" },
            { GameType.ClassicCompetitive, "+game_type 0 +game_mode 1" } ,
            { GameType.Deathmatch, "+game_type 1 +game_mode 2" }
        };
        /// <summary>
        /// The names of the map groups per game mode.
        /// </summary>
        public static Dictionary<GameType, string> MapGroups = new Dictionary<GameType, string>()
        {
            { GameType.ArmsRace, "mg_armsrace" },
            { GameType.Demolition, "mg_demolition" },
            { GameType.ClassicCasual, "mg_active" },
            { GameType.ClassicCompetitive, "mg_active" },
            { GameType.Deathmatch, "mg_deathmatch" },
        };
        /// <summary>
        /// The default map per game mode.
        /// </summary>
        public static Dictionary<GameType, string> DefaultMap = new Dictionary<GameType, string>()
        {
            { GameType.ArmsRace, "ar_baggage" },
            { GameType.Demolition, "de_safehouse" },
            { GameType.ClassicCasual, "de_nuke" },
            { GameType.ClassicCompetitive, "de_nuke" },
            { GameType.Deathmatch, "de_aztec" },
        };
        /// <summary>
        /// The defaults content for the server cfg-file.
        /// </summary>
        public static readonly List<string> DefaultServerCFGContent = new List<string>
        {
            "mp_freezetime 5",
            "mp_join_grace_time 15",
            "mp_match_end_restart 0",
            "mp_autokick 0",
            "sv_cheats 0",
            "sv_lan 0",
            "bot_difficulty 2",
            "bot_join_after_player 1",
            "bot_quota 10",
            "bot_quota_mode \"fill\"",
            "bot_defer_to_human_goals 0",
            "bot_defer_to_human_items 0",
            "exec banned_user.cfg",
            "exec banned_ip.cfg",
            "writeid",
            "writeip",
        };

        #endregion

        /// <summary>
        /// Creates a string containing the arguments for the specified setup.
        /// </summary>
        /// <param name="setup">The setup.</param>
        /// <returns>The argument string.</returns>
        public static string BuildDefaultArguments(BasicSetup setup)
        {
            return
                DEFAULT_ARGUMENTS +
                " " + GameTypeArguments[setup.GameType] +
                " +mapgroup " + MapGroups[setup.GameType] +
                " +map " + DefaultMap[setup.GameType] +
                " +hostport " + setup.Port +
                (string.IsNullOrWhiteSpace(setup.GSLT) ? "" : " +sv_setsteamaccount " + setup.GSLT + " -net_port_try 1") +
                DEFAULT_ARGUMENTS_TAIL +
                " -maxplayers_override " + setup.MaxPlayers +
                " -tickrate " + setup.TickRate;
        }

        /// <summary>
        /// Creates the server cfg from the setup.
        /// </summary>
        /// <param name="setup">The setup configuration.</param>
        public static void CreateServerCfg(BasicSetup setup)
        {
            // Create filename depending on gametype
            string serverCfgFilename = "";
            switch (setup.GameType)
            {
                case GameType.ArmsRace: serverCfgFilename = "gamemode_armsrace_server.cfg"; break;
                case GameType.Demolition: serverCfgFilename = "gamemode_demolition_server.cfg"; break;
                case GameType.ClassicCasual: serverCfgFilename = "gamemode_casual_server.cfg"; break;
                case GameType.ClassicCompetitive: serverCfgFilename = "gamemode_competitive_server.cfg"; break;
                case GameType.Deathmatch: serverCfgFilename = "gamemode_deathmatch_server.cfg"; break;
                default: throw new ArgumentException("Unknown gametype: " + setup.GameType.ToString());
            }

            // Create the corresponding cfg path
            string serverCfgPath = Path.Combine(setup.SRCDSPath, "csgo", "cfg", serverCfgFilename);

            // Write the server configuration file
            using (StreamWriter sw = new StreamWriter(serverCfgPath, false))
            {
                // Add base entries
                if (setup.Hostname != "")
                    sw.WriteLine("hostname \"" + setup.Hostname + "\"");
                else
                    sw.WriteLine("hostname \"CSGO Dedicated Server - launched by csgoDL\"");
                if (setup.RCONPW != "")
                    sw.WriteLine("rcon_password \"" + setup.RCONPW + "\"");
                if (setup.ServerPW != "")
                    sw.WriteLine("sv_password \"" + setup.ServerPW + "\"");

                // Add additional entries
                if (setup.ServerCFG != null)
                    foreach (var line in setup.ServerCFG)
                        sw.WriteLine(line);
            }
        }
        /// <summary>
        /// Creates the MOTD file for the dedicated server.
        /// </summary>
        /// <param name="setup">The setup.</param>
        public static void CreateMOTD(BasicSetup setup)
        {
            string path = Path.Combine(setup.SRCDSPath, "csgo", "motd.txt");
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine(setup.MOTD);
            }
        }
        /// <summary>
        /// Creates the game modes file respecting the given configuration.
        /// </summary>
        /// <param name="setup">Contains all necessary settings.</param>
        public static void CreateGamemodesFile(BasicSetup setup)
        {
            // Build the path to the gamemodes file
            string gamemodesPath = Path.Combine(setup.SRCDSPath, "csgo", "gamemodes_server.txt");

            // Write the gamemode configurations
            using (StreamWriter sw = new StreamWriter(gamemodesPath, false))
            {
                // Write intro
                sw.WriteLine("\"GameModes_Server.txt\"{");
                sw.WriteLine("\"gameTypes\"{");
                sw.WriteLine("\"classic\"{");
                sw.WriteLine("\"gameModes\"{");
                sw.WriteLine("\"competitive\"{");
                // ---> competitive config
                sw.WriteLine("}");
                sw.WriteLine("\"casual\"{");
                // ---> casual config
                sw.WriteLine("}}}\"gungame\"{\"gameModes\"{");
                sw.WriteLine("\"gungameprogressive\"{");
                // ---> arms race config
                if (setup.WeaponProgressionT != null && setup.WeaponProgressionCT != null && setup.WeaponProgressionT.Count() > 0 && setup.WeaponProgressionCT.Count() > 0)
                {
                    sw.WriteLine("\"maxplayers\" \"" + setup.MaxPlayers.ToString() + "\"");
                    sw.WriteLine("\"exec\"");
                    sw.WriteLine("{");
                    sw.WriteLine("\"exec\" \"server_last.cfg\"");
                    sw.WriteLine("}");
                    sw.WriteLine("\"mapgroupsMP\"");
                    sw.WriteLine("{");
                    sw.WriteLine("\"mg_armsrace\" \"\"");
                    sw.WriteLine("}");
                    sw.WriteLine("\"weaponprogression_ct\"");
                    sw.WriteLine("{");
                    foreach (var weapon in setup.WeaponProgressionCT)
                    {
                        sw.WriteLine("\"" + weapon.Type.ToString() + "\" { \"kills\" \"" + weapon.Count.ToString() + "\" }");
                    }
                    sw.WriteLine("\"knifegg\" { \"kills\" \"1\" }");
                    sw.WriteLine("}");
                    sw.WriteLine("\"weaponprogression_t\"");
                    sw.WriteLine("{");
                    foreach (var weapon in setup.WeaponProgressionT)
                    {
                        sw.WriteLine("\"" + weapon.Type.ToString() + "\" { \"kills\" \"" + weapon.Count.ToString() + "\" }");
                    }
                    sw.WriteLine("\"knifegg\" { \"kills\" \"1\" }");
                    sw.WriteLine("}");
                }
                sw.WriteLine("}");
                sw.WriteLine("\"gungametrbomb\"");
                sw.WriteLine("{");
                // ---> demolition config
                if (setup.WeaponProgressionT != null && setup.WeaponProgressionCT != null && setup.WeaponProgressionT.Count() > 0 && setup.WeaponProgressionCT.Count() > 0)
                {
                    sw.WriteLine("\"maxplayers\" \"" + setup.MaxPlayers.ToString() + "\"");
                    sw.WriteLine("\"exec\"");
                    sw.WriteLine("{");
                    sw.WriteLine("\"exec\" \"server_last.cfg\"");
                    sw.WriteLine("}");
                    sw.WriteLine("\"mapgroupsMP\"");
                    sw.WriteLine("{");
                    sw.WriteLine("\"mg_demolition\" \"\"");
                    sw.WriteLine("}");
                    sw.WriteLine("\"weaponprogression_ct\"");
                    sw.WriteLine("{");
                    foreach (var weapon in setup.WeaponProgressionCT)
                    {
                        sw.WriteLine("\"" + weapon.Type.ToString() + "\" { \"kills\" \"" + weapon.Count.ToString() + "\" }");
                    }
                    sw.WriteLine("}");
                    sw.WriteLine("\"weaponprogression_t\"");
                    sw.WriteLine("{");
                    foreach (var weapon in setup.WeaponProgressionT)
                    {
                        sw.WriteLine("\"" + weapon.Type.ToString() + "\" { \"kills\" \"" + weapon.Count.ToString() + "\" }");
                    }
                    sw.WriteLine("}");
                }
                sw.WriteLine("}}}}");
                sw.WriteLine("\"mapgroups\"");
                sw.WriteLine("{");
                // ---> mapgroup definitions
                sw.WriteLine("}");
                sw.WriteLine("}");
                sw.WriteLine("");
                sw.WriteLine("");
            }
        }
    }
}
