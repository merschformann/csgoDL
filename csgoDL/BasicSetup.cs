using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csgoDL
{
    /// <summary>
    /// Stores basic setup information for a CSGO server
    /// </summary>
    public class BasicSetup
    {
        #region I/O constants

        /// <summary>
        /// Used to separate command and value
        /// </summary>
        public const char DELIMETER = '=';
        /// <summary>
        /// Indicates the start of a block
        /// </summary>
        public const string BLOCK_BEGIN = "#";
        /// <summary>
        /// Indicates that this line is a comment
        /// </summary>
        public const string COMMENT = "//";

        /// <summary>
        /// Identifier of the block storing the basic settings
        /// </summary>
        public const string BLOCK_IDENT_BASICS = "Basics";
        /// <summary>
        /// Identifier of the T team's weapon progression block
        /// </summary>
        public const string BLOCK_IDENT_WEAPONPROGRESSION_T = "WeaponProgressionT";
        /// <summary>
        /// Identifier of the CT team's weapon progression block
        /// </summary>
        public const string BLOCK_IDENT_WEAPONPROGRESSION_CT = "WeaponProgressionCT";
        /// <summary>
        /// Identifier of the MOTD block
        /// </summary>
        public const string BLOCK_IDENT_MOTD = "MOTD";
        /// <summary>
        /// Identifier of the server cfg block.
        /// </summary>
        public const string BLOCK_IDENT_SERVER_CFG = "ServerCFG";

        #endregion

        /// <summary>
        /// The path to the SRCDS executable
        /// </summary>
        public string SRCDSPath { get; set; }

        /// <summary>
        /// The hostname of the server
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port of the server
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// The game server login token to use for the server.
        /// </summary>
        public string GSLT { get; set; }

        /// <summary>
        /// The pw to restrict access to the server (leave empty to disable restricted access)
        /// </summary>
        public string ServerPW { get; set; }

        /// <summary>
        /// The pw for remote administration
        /// </summary>
        public string RCONPW { get; set; }

        /// <summary>
        /// The gametype to play
        /// </summary>
        public GameType GameType { get; set; }

        /// <summary>
        /// The maximal number of players
        /// </summary>
        public string MaxPlayers { get; set; }

        /// <summary>
        /// The tickrate of the server
        /// </summary>
        public string TickRate { get; set; }

        /// <summary>
        /// The weapon progression for the Terror team (as used in Arms Race and Demolition)
        /// </summary>
        public List<WeaponProgressionElement> WeaponProgressionT { get; set; }

        /// <summary>
        /// The weapon progression for the Counter-Terror team (as used in Arms Race and Demolition)
        /// </summary>
        public List<WeaponProgressionElement> WeaponProgressionCT { get; set; }

        /// <summary>
        /// The MOTD for the server
        /// </summary>
        public string MOTD { get; set; }

        /// <summary>
        /// Additional server settings that are written to the cfg.
        /// </summary>
        public List<string> ServerCFG { get; set; }

        /// <summary>
        /// Reads the setup data from a file
        /// </summary>
        /// <param name="filepath">The path to a setup file</param>
        /// <returns></returns>
        public void ReadFile(string filepath)
        {
            // Read setup file
            Dictionary<string, List<string>> blocks = new Dictionary<string, List<string>>();
            using (StreamReader sr = new StreamReader(filepath))
            {
                // Read blocks
                string currentBlock = "";
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    // Skip comments
                    if (line.StartsWith(COMMENT) || string.IsNullOrWhiteSpace(line)) { continue; }
                    // If it is a block start to set everything up
                    if (line.StartsWith(BLOCK_BEGIN))
                    {
                        if (line.Contains(BLOCK_IDENT_BASICS)) { blocks[BLOCK_IDENT_BASICS] = new List<string>(); currentBlock = BLOCK_IDENT_BASICS; continue; }
                        else if (line.Contains(BLOCK_IDENT_WEAPONPROGRESSION_T)) { blocks[BLOCK_IDENT_WEAPONPROGRESSION_T] = new List<string>(); currentBlock = BLOCK_IDENT_WEAPONPROGRESSION_T; continue; }
                        else if (line.Contains(BLOCK_IDENT_WEAPONPROGRESSION_CT)) { blocks[BLOCK_IDENT_WEAPONPROGRESSION_CT] = new List<string>(); currentBlock = BLOCK_IDENT_WEAPONPROGRESSION_CT; continue; }
                        else if (line.Contains(BLOCK_IDENT_MOTD)) { blocks[BLOCK_IDENT_MOTD] = new List<string>(); currentBlock = BLOCK_IDENT_MOTD; continue; }
                        else if (line.Contains(BLOCK_IDENT_SERVER_CFG)) { blocks[BLOCK_IDENT_SERVER_CFG] = new List<string>(); currentBlock = BLOCK_IDENT_SERVER_CFG; continue; }
                        else { continue; }
                    }
                    // Store every line of block
                    blocks[currentBlock].Add(line);
                }
            }

            // --> Basics
            BasicSetup setup = new BasicSetup();
            Dictionary<string, string> entries = new Dictionary<string, string>();

            // Read block
            foreach (var line in blocks[BLOCK_IDENT_BASICS])
            {
                if (line.StartsWith(COMMENT)) { continue; }
                if (line.Contains(DELIMETER))
                {
                    string[] eles = line.Split(DELIMETER);
                    entries[eles[0]] = eles[1];
                }
            }

            // Set fields
            SRCDSPath = entries[Helper.Check(() => SRCDSPath)];
            Hostname = entries[Helper.Check(() => Hostname)];
            Port = entries[Helper.Check(() => Port)];
            GSLT = entries[Helper.Check(() => GSLT)];
            ServerPW = entries[Helper.Check(() => ServerPW)];
            RCONPW = entries[Helper.Check(() => RCONPW)];
            GameType = (GameType)Enum.Parse(typeof(GameType), entries[Helper.Check(() => GameType)]);
            MaxPlayers = entries[Helper.Check(() => MaxPlayers)];
            TickRate = entries[Helper.Check(() => TickRate)];

            // --> Weapon progression T
            WeaponProgressionT = new List<WeaponProgressionElement>();
            foreach (var line in blocks[BLOCK_IDENT_WEAPONPROGRESSION_T])
            {
                if (line.StartsWith(COMMENT)) { continue; }
                if (line.Contains(DELIMETER))
                {
                    string[] eles = line.Split(DELIMETER);
                    if (eles.Count() == 2)
                    {
                        WeaponType type = (WeaponType)Enum.Parse(typeof(WeaponType), eles[0]);
                        int count = int.Parse(eles[1]);
                        WeaponProgressionT.Add(new WeaponProgressionElement() { Type = type, Count = count });
                    }
                }
            }

            // --> Weapon progression CT
            WeaponProgressionCT = new List<WeaponProgressionElement>();
            foreach (var line in blocks[BLOCK_IDENT_WEAPONPROGRESSION_CT])
            {
                if (line.StartsWith(COMMENT)) { continue; }
                if (line.Contains(DELIMETER))
                {
                    string[] eles = line.Split(DELIMETER);
                    if (eles.Count() == 2)
                    {
                        WeaponType type = (WeaponType)Enum.Parse(typeof(WeaponType), eles[0]);
                        int count = int.Parse(eles[1]);
                        WeaponProgressionCT.Add(new WeaponProgressionElement() { Type = type, Count = count });
                    }
                }
            }

            // --> MOTD
            MOTD = string.Join(Environment.NewLine, blocks[BLOCK_IDENT_MOTD]);

            // --> Server cfg
            ServerCFG = blocks[BLOCK_IDENT_SERVER_CFG].ToList();
        }

        /// <summary>
        /// Writes the setup data to a simple file format
        /// </summary>
        /// <param name="filepath">The path to the file</param>
        public void WriteFile(string filepath)
        {
            // Write the content to a simple file
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                // --> Basics
                sw.WriteLine(BLOCK_BEGIN + " " + BLOCK_IDENT_BASICS);
                sw.WriteLine(Helper.Check(() => SRCDSPath) + DELIMETER + SRCDSPath);
                sw.WriteLine(Helper.Check(() => Hostname) + DELIMETER + Hostname);
                sw.WriteLine(Helper.Check(() => Port) + DELIMETER + Port);
                sw.WriteLine(Helper.Check(() => GSLT) + DELIMETER + GSLT);
                sw.WriteLine(Helper.Check(() => ServerPW) + DELIMETER + ServerPW);
                sw.WriteLine(Helper.Check(() => RCONPW) + DELIMETER + RCONPW);
                sw.WriteLine(Helper.Check(() => GameType) + DELIMETER + GameType.ToString());
                sw.WriteLine(Helper.Check(() => MaxPlayers) + DELIMETER + MaxPlayers);
                sw.WriteLine(Helper.Check(() => TickRate) + DELIMETER + TickRate);

                // --> Weapon progression T
                sw.WriteLine(BLOCK_BEGIN + " " + BLOCK_IDENT_WEAPONPROGRESSION_T);
                foreach (var weapon in WeaponProgressionT)
                {
                    sw.WriteLine(weapon.Type.ToString() + DELIMETER + weapon.Count.ToString());
                }
                // --> Weapon progression CT
                sw.WriteLine(BLOCK_BEGIN + " " + BLOCK_IDENT_WEAPONPROGRESSION_CT);
                foreach (var weapon in WeaponProgressionCT)
                {
                    sw.WriteLine(weapon.Type.ToString() + DELIMETER + weapon.Count.ToString());
                }
                // --> MOTD
                sw.WriteLine(BLOCK_BEGIN + " " + BLOCK_IDENT_MOTD);
                sw.WriteLine(MOTD);
                // --> ServerCFG
                sw.WriteLine(BLOCK_BEGIN + " " + BLOCK_IDENT_SERVER_CFG);
                sw.WriteLine(string.Join(Environment.NewLine, ServerCFG));
            }
        }
    }
}
