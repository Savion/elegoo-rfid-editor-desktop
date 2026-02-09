using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDEditor
{
    public class ElegooSpool
    {
        public byte[] RawData { get; private set; }

        // Added verified Internal Base to ensure "New" files have the correct structure
        private static readonly byte[] InternalBase = new byte[180]
        {
            0x53, 0x44, 0xE5, 0x7A, 0x01, 0xA0, 0x00, 0x04, 0xA5, 0x48, 0x00, 0x00, 0xE1, 0x10, 0x12, 0x00,
            0x01, 0x03, 0xA0, 0x0C, 0x34, 0x03, 0x0F, 0xD1, 0x01, 0x0B, 0x55, 0x02, 0x65, 0x6C, 0x65, 0x67,
            0x6F, 0x6F, 0x2E, 0x63, 0x6F, 0x6D, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x36, 0xEE, 0xEE, 0xEE, 0xEE, 0x00, 0x00, 0x00, 0x00, 0x80, 0x76, 0x65, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xFF, 0x00, 0xBE, 0x00, 0xE6, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAF, 0x03, 0xE8,
            0x00, 0x36, 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xBD, 0x04, 0x00, 0x00
        };

        public void GenerateNewIdentity()
        {
            Random rng = new();

            // 1. Generate 7 random bytes for the Hardware UID
            byte[] newUid = new byte[7];
            rng.NextBytes(newUid);

            // 2. Patch Page 0: [UID0] [UID1] [UID2] [BCC0]
            // BCC0 is defined as 0x88 ^ UID0 ^ UID1 ^ UID2
            RawData[0] = newUid[0];
            RawData[1] = newUid[1];
            RawData[2] = newUid[2];
            RawData[3] = (byte)(0x88 ^ RawData[0] ^ RawData[1] ^ RawData[2]);

            // 3. Patch Page 1: [UID3] [UID4] [UID5] [UID6]
            RawData[4] = newUid[3];
            RawData[5] = newUid[4];
            RawData[6] = newUid[5];
            RawData[7] = newUid[6];

            // 4. Patch Page 2, Byte 0: [BCC1]
            // BCC1 is defined as UID3 ^ UID4 ^ UID5 ^ UID6
            RawData[8] = (byte)(RawData[4] ^ RawData[5] ^ RawData[6] ^ RawData[7]);

        }

        public ElegooSpool(byte[] data)
        {
            // Fix: If array is empty or null, use the InternalBase
            if (data == null || data.Length == 0 || data.All(b => b == 0))
            {
                RawData = (byte[])InternalBase.Clone();
            }
            else
            {
                if (data.Length < 180) Array.Resize(ref data, 180);
                RawData = data;
            }
        }

        // Complete material type codes from verified RFID spec
        private static readonly Dictionary<string, uint> MaterialCodes = new()
        {
            ["PLA"] = 0x00807665,
            ["PETG"] = 0x80698471,
            ["ABS"] = 0x00656683,
            ["TPU"] = 0x00848085,
            ["PA"] = 0x00008065,
            ["CPE"] = 0x00678069,
            ["PC"] = 0x00008067,
            ["PVA"] = 0x00808665,
            ["ASA"] = 0x00658365,
            ["BVOH"] = 0x42564F48,
            ["EVA"] = 0x00455641,
            ["HIPS"] = 0x48495053,
            ["PP"] = 0x00005050,
            ["PPA"] = 0x00505041,
            ["PPS"] = 0x00505053
        };

        // Complete subtype codes from verified RFID spec
        private static readonly Dictionary<ushort, string> SubtypeCodes = new()
        {
            // PLA Family (0x00XX)
            [0x0000] = "PLA",
            [0x0001] = "PLA+",
            [0x0002] = "PLA Pro",
            [0x0003] = "PLA Silk",
            [0x0004] = "PLA-CF",
            [0x0005] = "PLA Carbon",
            [0x0006] = "PLA Matte",
            [0x0007] = "PLA Fluo",
            [0x0008] = "PLA Wood",
            [0x0009] = "PLA Basic",
            [0x000A] = "RAPID PLA+",
            [0x000B] = "PLA Marble",
            [0x000C] = "PLA Galaxy",
            [0x000D] = "PLA Red Copper",
            [0x000E] = "PLA Sparkle",
            // PETG Family (0x01XX)
            [0x0100] = "PETG",
            [0x0101] = "PETG-CF",
            [0x0102] = "PETG-GF",
            [0x0103] = "PETG Pro",
            [0x0104] = "PETG Translucent",
            [0x0105] = "RAPID PETG",
            // ABS Family (0x02XX)
            [0x0200] = "ABS",
            [0x0201] = "ABS-GF",
            // TPU Family (0x03XX)
            [0x0300] = "TPU",
            [0x0301] = "TPU 95A",
            [0x0302] = "RAPID TPU 95A",
            // PA Family (0x04XX)
            [0x0400] = "PA",
            [0x0401] = "PA-CF",
            [0x0403] = "PAHT-CF",
            [0x0404] = "PA6",
            [0x0405] = "PA6-CF",
            [0x0406] = "PA12",
            [0x0407] = "PA12-CF",
            // Other Materials
            [0x0500] = "CPE",
            [0x0600] = "PC",
            [0x0601] = "PCTG",
            [0x0602] = "PC-FR",
            [0x0700] = "PVA",
            [0x0800] = "ASA",
            [0x0900] = "BVOH",
            [0x0A00] = "EVA",
            [0x0B00] = "HIPS",
            [0x0C00] = "PP",
            [0x0C01] = "PP-CF",
            [0x0C02] = "PP-GF",
            [0x0D00] = "PPA",
            [0x0D01] = "PPA-CF",
            [0x0D02] = "PPA-GF",
            [0x0E00] = "PPS",
            [0x0E02] = "PPS-CF"
        };

        public static IEnumerable<string> GetAllMaterials() => MaterialCodes.Keys.OrderBy(k => k);

        public static IEnumerable<string> GetSubtypesForMaterial(string material)
        {
            byte familyCode = material switch
            {
                "PLA" => 0x00,
                "PETG" => 0x01,
                "ABS" => 0x02,
                "TPU" => 0x03,
                "PA" => 0x04,
                "CPE" => 0x05,
                "PC" => 0x06,
                "PVA" => 0x07,
                "ASA" => 0x08,
                "BVOH" => 0x09,
                "EVA" => 0x0A,
                "HIPS" => 0x0B,
                "PP" => 0x0C,
                "PPA" => 0x0D,
                "PPS" => 0x0E,
                _ => 0xFF
            };

            if (familyCode == 0xFF) return Enumerable.Empty<string>();

            return SubtypeCodes
                .Where(kvp => (kvp.Key >> 8) == familyCode)
                .Select(kvp => kvp.Value)
                .OrderBy(v => v);
        }

        public string Material
        {
            get
            {
                uint val = (uint)((RawData[0x48] << 24) | (RawData[0x49] << 16) | (RawData[0x4A] << 8) | RawData[0x4B]);
                return MaterialCodes.FirstOrDefault(kvp => kvp.Value == val).Key ?? "Unknown";
            }
            set
            {
                if (MaterialCodes.TryGetValue(value, out uint signature))
                {
                    RawData[0x48] = (byte)(signature >> 24);
                    RawData[0x49] = (byte)(signature >> 16);
                    RawData[0x4A] = (byte)(signature >> 8);
                    RawData[0x4B] = (byte)(signature & 0xFF);
                }
            }
        }

        // Material Subtype using 2-byte encoding (0x4C-0x4D)
        public ushort MaterialSubtypeCode
        {
            get => (ushort)((RawData[0x4C] << 8) | RawData[0x4D]);
            set
            {
                RawData[0x4C] = (byte)(value >> 8);
                RawData[0x4D] = (byte)(value & 0xFF);
            }
        }

        public string MaterialSubtype
        {
            get
            {
                ushort code = MaterialSubtypeCode;
                return SubtypeCodes.TryGetValue(code, out string? name) ? name : $"Unknown (0x{code:X4})";
            }
            set
            {
                var kvp = SubtypeCodes.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
                if (kvp.Value != null)
                {
                    MaterialSubtypeCode = kvp.Key;
                }
            }
        }

        /*The color category/brightness system (PR/SE/AC/MT/GL/TR and L/M/D) from the GitHub guide is NOT used in the Canvas firmware.*/
        public char ColorModifier
        {
            get => (char)RawData[0x53];
            set => RawData[0x53] = (byte)value;
        }

        public int Weight
        {
            get => (RawData[0x5E] << 8) | RawData[0x5F];
            set { RawData[0x5E] = (byte)(value >> 8); RawData[0x5F] = (byte)(value & 0xFF); }
        }

        public int Diameter
        {
            get => (RawData[0x5C] << 8) | RawData[0x5D];
            set { RawData[0x5C] = (byte)(value >> 8); RawData[0x5D] = (byte)(value & 0xFF); }
        }

        public int MinTemp
        {
            get => (RawData[0x54] << 8) | RawData[0x55];
            set { RawData[0x54] = (byte)(value >> 8); RawData[0x55] = (byte)(value & 0xFF); }
        }

        public int MaxTemp
        {
            get => (RawData[0x56] << 8) | RawData[0x57];
            set { RawData[0x56] = (byte)(value >> 8); RawData[0x57] = (byte)(value & 0xFF); }
        }

        public string ProductionDate
        {
            get
            {
                // Combine 0x60 and 0x61 into a single 16-bit value
                int encodedDate = (RawData[0x60] << 8) | RawData[0x61];
                return encodedDate.ToString("D4"); // Returns "2502"
            }
            set
            {
                if (int.TryParse(value, out int encodedDate))
                {
                    // Store as Big-Endian: 2502 -> 0x09 (0x60) and 0xC6 (0x61)
                    RawData[0x60] = (byte)(encodedDate >> 8);
                    RawData[0x61] = (byte)(encodedDate & 0xFF);
                }
            }
        }

        public void FixChecksums()
        {
            RawData[0x08] = (byte)(RawData[0x04] ^ RawData[0x05] ^ RawData[0x06] ^ RawData[0x07]);
        }
    }
}