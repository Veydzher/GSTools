using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GSMdtTools.Decoders
{
    public class MdtDecoder : IDecoder
    {
        private readonly Stream mdtStream;

        GSOperation[] gsOps = {
            new GSOperation("CodeProc_00", 0),
            new GSOperation("CodeProc_01", 0),
            new GSOperation("CodeProc_02", 0),
            new GSOperation("CodeProc_03", 1),
            new GSOperation("CodeProc_04", 1),
            new GSOperation("CodeProc_05", 2),
            new GSOperation("CodeProc_06", 2),
            new GSOperation("CodeProc_02 (2)", 0),
            new GSOperation("CodeProc_08", 2),
            new GSOperation("CodeProc_09", 3),
            new GSOperation("CodeProc_02 (3)", 1),
            new GSOperation("CodeProc_0B", 1),
            new GSOperation("CodeProc_0C", 1),
            new GSOperation("CodeProc_0D", 0),
            new GSOperation("CodeProc_0E", 1),
            new GSOperation("CodeProc_0F", 2),
            new GSOperation("CodeProc_10", 1),
            new GSOperation("CodeProc_11", 0),
            new GSOperation("CodeProc_12", 3),
            new GSOperation("CodeProc_13", 1),
            new GSOperation("CodeProc_14", 0),
            new GSOperation("CodeProc_15", 0),
            new GSOperation("CodeProc_16", 0),
            new GSOperation("CodeProc_17", 1),
            new GSOperation("CodeProc_18", 1),
            new GSOperation("CodeProc_19", 2),
            new GSOperation("CodeProc_1A", 4),
            new GSOperation("CodeProc_1B", 1),
            new GSOperation("CodeProc_1C", 1),
            new GSOperation("CodeProc_1D", 1),
            new GSOperation("CodeProc_1E", 3),
            new GSOperation("CodeProc_1F", 0),
            new GSOperation("CodeProc_20", 1),
            new GSOperation("CodeProc_21", 0),
            new GSOperation("CodeProc_22", 2),
            new GSOperation("CodeProc_23", 2),
            new GSOperation("CodeProc_24", 0),
            new GSOperation("CodeProc_25", 1),
            new GSOperation("CodeProc_26", 1),
            new GSOperation("CodeProc_27", 2),
            new GSOperation("CodeProc_28", 1),
            new GSOperation("CodeProc_29", 1),
            new GSOperation("CodeProc_2A", 3),
            new GSOperation("CodeProc_2B", 0),
            new GSOperation("CodeProc_2C", 1),
            new GSOperation("CodeProc_02 (4)", 0),
            new GSOperation("CodeProc_2E", 0),
            new GSOperation("CodeProc_2F", 2),
            new GSOperation("CodeProc_30", 1),
            new GSOperation("CodeProc_31", 2),
            new GSOperation("CodeProc_32", 2),
            new GSOperation("CodeProc_33", 5),
            new GSOperation("CodeProc_34", 1),
            new GSOperation("CodeProc_35", 2),
            new GSOperation("CodeProc_36", 1),
            new GSOperation("CodeProc_37", 2),
            new GSOperation("CodeProc_38", 1),
            new GSOperation("CodeProc_39", 1),
            new GSOperation("CodeProc_3A", 3),
            new GSOperation("CodeProc_3B", 2),
            new GSOperation("CodeProc_3C", 1),
            new GSOperation("CodeProc_3D", 1),
            new GSOperation("CodeProc_3E", 1),
            new GSOperation("CodeProc_3F", 0),
            new GSOperation("CodeProc_40", 0),
            new GSOperation("CodeProc_41", 0),
            new GSOperation("CodeProc_42", 1),
            new GSOperation("CodeProc_43", 1),
            new GSOperation("CodeProc_44", 1),
            new GSOperation("CodeProc_15", 0),
            new GSOperation("CodeProc_46", 1),
            new GSOperation("CodeProc_47", 2),
            new GSOperation("CodeProc_48", 2),
            new GSOperation("CodeProc_49", 0),
            new GSOperation("CodeProc_4A", 1),
            new GSOperation("CodeProc_4B", 1),
            new GSOperation("CodeProc_4C", 0),
            new GSOperation("CodeProc_4D", 2),
            new GSOperation("CodeProc_4E", 1),
            new GSOperation("CodeProc_4F", 7),
            new GSOperation("CodeProc_50", 1),
            new GSOperation("CodeProc_51", 2),
            new GSOperation("CodeProc_52", 1),
            new GSOperation("CodeProc_53", 0),
            new GSOperation("CodeProc_54", 2),
            new GSOperation("CodeProc_55", 1),
            new GSOperation("CodeProc_56", 2),
            new GSOperation("CodeProc_57", 1),
            new GSOperation("CodeProc_58", 0),
            new GSOperation("CodeProc_59", 1),
            new GSOperation("CodeProc_5A", 1),
            new GSOperation("CodeProc_5B", 2),
            new GSOperation("CodeProc_5C", 3),
            new GSOperation("CodeProc_5D", 0),
            new GSOperation("CodeProc_5E", 0),
            new GSOperation("CodeProc_5F", 3),
            new GSOperation("CodeProc_60", 4),
            new GSOperation("CodeProc_61", 3),
            new GSOperation("CodeProc_62", 0),
            new GSOperation("CodeProc_63", 0),
            new GSOperation("CodeProc_64", 1),
            new GSOperation("CodeProc_65", 2),
            new GSOperation("CodeProc_66", 3),
            new GSOperation("CodeProc_67", 0),
            new GSOperation("CodeProc_68", 0),
            new GSOperation("CodeProc_69", 4),
            new GSOperation("CodeProc_6A", 1),
            new GSOperation("CodeProc_6B", 3),
            new GSOperation("CodeProc_6C", 0),
            new GSOperation("CodeProc_6D", 1),
            new GSOperation("CodeProc_6E", 1),
            new GSOperation("CodeProc_6F", 1),
            new GSOperation("CodeProc_70", 3),
            new GSOperation("CodeProc_71", 3),
            new GSOperation("CodeProc_DUMMY", 0),
            new GSOperation("CodeProc_DUMMY", 0),
            new GSOperation("CodeProc_74", 2),
            new GSOperation("CodeProc_75", 4),
            new GSOperation("CodeProc_76", 2),
            new GSOperation("CodeProc_77", 2),
            new GSOperation("CodeProc_36", 1),
            new GSOperation("CodeProc_15", 0),
            new GSOperation("CodeProc_7A", 1),
            new GSOperation("CodeProc_7B", 2),
            new GSOperation("CodeProc_7C", 0),
            new GSOperation("CodeProc_7D", 1),
            new GSOperation("CodeProc_7E", 1),
            new GSOperation("CodeProc_7F", 1),
        };

        Dictionary<string, string> EnToHalfMap = new Dictionary<string, string>
        {
            {
                "１",
                "1"
            },
            {
                "２",
                "2"
            },
            {
                "３",
                "3"
            },
            {
                "４",
                "4"
            },
            {
                "５",
                "5"
            },
            {
                "６",
                "6"
            },
            {
                "７",
                "7"
            },
            {
                "８",
                "8"
            },
            {
                "９",
                "9"
            },
            {
                "０",
                "0"
            },
            {
                "Ａ",
                "A"
            },
            {
                "Ｂ",
                "B"
            },
            {
                "Ｃ",
                "C"
            },
            {
                "Ｄ",
                "D"
            },
            {
                "Ｅ",
                "E"
            },
            {
                "Ｆ",
                "F"
            },
            {
                "Ｇ",
                "G"
            },
            {
                "Ｈ",
                "H"
            },
            {
                "Ｉ",
                "I"
            },
            {
                "Ｊ",
                "J"
            },
            {
                "Ｋ",
                "K"
            },
            {
                "Ｌ",
                "L"
            },
            {
                "Ｍ",
                "M"
            },
            {
                "Ｎ",
                "N"
            },
            {
                "Ｏ",
                "O"
            },
            {
                "Ｐ",
                "P"
            },
            {
                "Ｑ",
                "Q"
            },
            {
                "Ｒ",
                "R"
            },
            {
                "Ｓ",
                "S"
            },
            {
                "Ｔ",
                "T"
            },
            {
                "Ｕ",
                "U"
            },
            {
                "Ｖ",
                "V"
            },
            {
                "Ｗ",
                "W"
            },
            {
                "Ｘ",
                "X"
            },
            {
                "Ｙ",
                "Y"
            },
            {
                "Ｚ",
                "Z"
            },
            {
                "ａ",
                "a"
            },
            {
                "ｂ",
                "b"
            },
            {
                "ｃ",
                "c"
            },
            {
                "ｄ",
                "d"
            },
            {
                "ｅ",
                "e"
            },
            {
                "ｆ",
                "f"
            },
            {
                "ｇ",
                "g"
            },
            {
                "ｈ",
                "h"
            },
            {
                "ｉ",
                "i"
            },
            {
                "ｊ",
                "j"
            },
            {
                "ｋ",
                "k"
            },
            {
                "ｌ",
                "l"
            },
            {
                "ｍ",
                "m"
            },
            {
                "ｎ",
                "n"
            },
            {
                "ｏ",
                "o"
            },
            {
                "ｐ",
                "p"
            },
            {
                "ｑ",
                "q"
            },
            {
                "ｒ",
                "r"
            },
            {
                "ｓ",
                "s"
            },
            {
                "ｔ",
                "t"
            },
            {
                "ｕ",
                "u"
            },
            {
                "ｖ",
                "v"
            },
            {
                "ｗ",
                "w"
            },
            {
                "ｘ",
                "x"
            },
            {
                "ｙ",
                "y"
            },
            {
                "ｚ",
                "z"
            },
            {
                "\u3000",
                " "
            },
            {
                "．",
                "."
            },
            {
                "，",
                ","
            },
            {
                "＇",
                "'"
            },
            {
                "！",
                "!"
            },
            {
                "（",
                "("
            },
            {
                "）",
                ")"
            },
            {
                "－",
                "-"
            },
            {
                "／",
                "/"
            },
            {
                "？",
                "?"
            },
            {
                "∠",
                "_"
            },
            {
                "［",
                "["
            },
            {
                "］",
                "]"
            },
            {
                "“",
                "\""
            },
            {
                "”",
                "\""
            },
            {
                "＂",
                "\""
            },
            {
                "―",
                "-"
            },
            {
                "‘",
                "'"
            },
            {
                "’",
                "'"
            },
            {
                "：",
                ":"
            },
            {
                "＊",
                "*"
            },
            {
                "；",
                ";"
            },
            {
                "＄",
                "$"
            },
            {
                "Ы",
                "©"
            },
            {
                "∋",
                "è"
            },
            {
                "∈",
                "é"
            },
            {
                "∀",
                "á"
            },
            {
                "∧",
                "à"
            },
            {
                "⊆",
                "ç"
            },
            {
                "⊂",
                "Ç"
            },
            {
                "Ц",
                "û"
            },
            {
                "↑",
                "î"
            },
            {
                "α",
                "â"
            },
            {
                "л",
                "ñ"
            },
            {
                "↓",
                "ï"
            },
            {
                "ε",
                "ê"
            }
        };

        public MdtDecoder(Stream inputStream)
        {
            if (inputStream.CanRead)
            {
                mdtStream = inputStream;
            }
            else
            {
                throw new Exception("MDT Stream is not readable");
            }
        }

        public enum MdtEntryKind
        {
            Message,
            Label
        }

        public sealed class MdtEntry
        {
            public ushort Index { get; set; }
            public uint RawValue { get; set; }
            public MdtEntryKind Kind { get; set; }

            // For Message entries
            public uint MessageWordOffset { get; set; }

            // For Label entries
            public ushort TargetMessageIndex { get; set; }
            public ushort ByteOffsetWithinMessage { get; set; }
            public uint LabelWordOffset { get; set; }
        }

        private void DecodeMessage(ushort[] messageData, uint start, uint end, List<IGSToken> tokens)
        {
            var sb = new StringBuilder();
            uint pos = start;

            while (pos < end)
            {
                ushort word = messageData[pos];

                if (word >= 128)
                {
                    ushort ch = (ushort)(word - 128);
                    sb.Append(EnToHalfMap.TryGetValue(char.ConvertFromUtf32(ch), out string mapped) ? mapped : char.ConvertFromUtf32(ch));
                    pos++;
                    continue;
                }

                if (sb.Length > 0)
                {
                    tokens.Add(new GSStringToken(sb.ToString()));
                    sb.Clear();
                }

                ushort opcode = word;

                if (opcode >= gsOps.Length)
                {
                    throw new InvalidDataException($"Unknown opcode 0x{opcode:X4} at message word index {pos}");
                }

                ushort argCount = gsOps[opcode].ArgCount;

                if (pos + argCount > end)
                {
                    throw new InvalidDataException(
                        $"Opcode 0x{opcode:X4} at word index {pos} exceeds message boundary"
                    );
                }

                ushort[] args = new ushort[argCount];
                for (ushort i = 0; i < argCount; i++)
                {
                    args[i] = messageData[pos + 1 + i];
                }

                tokens.Add(new GSOperationToken(gsOps[opcode].Name, opcode, args));
                pos += (uint)(1 + argCount);
            }

            if (sb.Length > 0)
            {
                tokens.Add(new GSStringToken(sb.ToString()));
            }
        }

        private List<MdtEntry> ClassifyEntries(uint[] rawOffsets, uint fileMessageOffset, uint messageDataWordCount)
        {
            var entries = new List<MdtEntry>(rawOffsets.Length);

            for (ushort i = 0; i < rawOffsets.Length; i++)
            {
                uint raw = rawOffsets[i];

                uint wordOffset;
                bool looksLikeMessage = false;

                if (raw >= fileMessageOffset)
                {
                    wordOffset = (raw - fileMessageOffset) / 2;
                    looksLikeMessage = wordOffset < messageDataWordCount;
                }

                if (looksLikeMessage)
                {
                    entries.Add(new MdtEntry
                    {
                        Index = i,
                        RawValue = raw,
                        Kind = MdtEntryKind.Message,
                        MessageWordOffset = (raw - fileMessageOffset) / 2
                    });
                }
                else
                {
                    ushort targetMessage = (ushort)(raw >> 16);
                    ushort byteOffsetWithinMessage = (ushort)(raw & 0xFFFF);

                    uint targetMessageWordOffset = 0;
                    if (targetMessage < rawOffsets.Length)
                    {
                        uint targetRaw = rawOffsets[targetMessage];
                        if (targetRaw >= fileMessageOffset)
                        {
                            targetMessageWordOffset = (targetRaw - fileMessageOffset) / 2;
                        }
                    }

                    entries.Add(new MdtEntry
                    {
                        Index = i,
                        RawValue = raw,
                        Kind = MdtEntryKind.Label,
                        TargetMessageIndex = targetMessage,
                        ByteOffsetWithinMessage = byteOffsetWithinMessage,
                        LabelWordOffset = targetMessageWordOffset + (uint)(byteOffsetWithinMessage / 2)
                    });
                }
            }

            return entries;
        }

        public List<IGSToken> DecodeStream()
        {
            var tokens = new List<IGSToken>();

            using var reader = new BinaryReader(mdtStream, Encoding.UTF8, leaveOpen: true);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            ushort messageCount = reader.ReadUInt16();
            ushort dummy = reader.ReadUInt16();
            uint fileMessageOffset = (uint)(4 + messageCount * 4);

            uint[] rawOffsets = new uint[messageCount];
            for (int i = 0; i < messageCount; i++)
                rawOffsets[i] = reader.ReadUInt32();

            ushort[] messageData = new ushort[(reader.BaseStream.Length - reader.BaseStream.Position) / 2];
            for (int i = 0; i < messageData.Length; i++)
                messageData[i] = reader.ReadUInt16();

            var entries = ClassifyEntries(rawOffsets, fileMessageOffset, (uint)messageData.Length);
            var messageEntries = entries
                .Where(e => e.Kind == MdtEntryKind.Message)
                .OrderBy(e => e.MessageWordOffset)
                .ToList();

            tokens.Add(new GSMessageCountToken(messageCount));

            foreach (var entry in entries.Where(e => e.Kind == MdtEntryKind.Label))
            {
                tokens.Add(new GSLabelToken(entry.Index, entry.TargetMessageIndex, entry.ByteOffsetWithinMessage));
            }

            for (int i = 0; i < messageEntries.Count; i++)
            {
                uint start = messageEntries[i].MessageWordOffset;
                uint end = (i + 1 < messageEntries.Count)
                    ? messageEntries[i + 1].MessageWordOffset
                    : (uint)messageData.Length;

                DecodeMessage(messageData, start, end, tokens);
                tokens.Add(new GSEndMessageToken());
            }

            return tokens;
        }
    }
}
