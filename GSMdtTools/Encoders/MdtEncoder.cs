using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GSMdtTools.Encoders
{
    class MdtEncoder : IEncoder
    {
        private readonly List<IGSToken> tokens;
        private Stream outputStream;

        private static readonly Dictionary<string, string> HalfToFullMap = new Dictionary<string, string>
        {
            { "1", "１" },
            { "2", "２" },
            { "3", "３" },
            { "4", "４" },
            { "5", "５" },
            { "6", "６" },
            { "7", "７" },
            { "8", "８" },
            { "9", "９" },
            { "0", "０" },

            { "A", "Ａ" },
            { "B", "Ｂ" },
            { "C", "Ｃ" },
            { "D", "Ｄ" },
            { "E", "Ｅ" },
            { "F", "Ｆ" },
            { "G", "Ｇ" },
            { "H", "Ｈ" },
            { "I", "Ｉ" },
            { "J", "Ｊ" },
            { "K", "Ｋ" },
            { "L", "Ｌ" },
            { "M", "Ｍ" },
            { "N", "Ｎ" },
            { "O", "Ｏ" },
            { "P", "Ｐ" },
            { "Q", "Ｑ" },
            { "R", "Ｒ" },
            { "S", "Ｓ" },
            { "T", "Ｔ" },
            { "U", "Ｕ" },
            { "V", "Ｖ" },
            { "W", "Ｗ" },
            { "X", "Ｘ" },
            { "Y", "Ｙ" },
            { "Z", "Ｚ" },

            { "a", "ａ" },
            { "b", "ｂ" },
            { "c", "ｃ" },
            { "d", "ｄ" },
            { "e", "ｅ" },
            { "f", "ｆ" },
            { "g", "ｇ" },
            { "h", "ｈ" },
            { "i", "ｉ" },
            { "j", "ｊ" },
            { "k", "ｋ" },
            { "l", "ｌ" },
            { "m", "ｍ" },
            { "n", "ｎ" },
            { "o", "ｏ" },
            { "p", "ｐ" },
            { "q", "ｑ" },
            { "r", "ｒ" },
            { "s", "ｓ" },
            { "t", "ｔ" },
            { "u", "ｕ" },
            { "v", "ｖ" },
            { "w", "ｗ" },
            { "x", "ｘ" },
            { "y", "ｙ" },
            { "z", "ｚ" },

            { " ", "\u3000" },
            { ".", "．" },
            { ",", "，" },
            { "'", "＇" },
            { "!", "！" },
            { "(", "（" },
            { ")", "）" },
            { "/", "／" },
            { "?", "？" },
            { "_", "∠" },
            { "[", "［" },
            { "]", "］" },
            { "\"", "＂" },
            { "-", "―" },
            { ":", "：" },
            { "*", "＊" },
            { ";", "；" },
            { "$", "＄" },
            { "©", "Ы" },
            { "è", "∋" },
            { "é", "∈" },
            { "á", "∀" },
            { "à", "∧" },
            { "ç", "⊆" },
            { "Ç", "⊂" },
            { "û", "Ц" },
            { "î", "↑" },
            { "â", "α" },
            { "ñ", "л" },
            { "ï", "↓" },
            { "ê", "ε" },
            { "&", "＆" }
        };

        public MdtEncoder(Stream outputStream, List<IGSToken> tokens)
        {
            if (!outputStream.CanWrite)
                throw new InvalidOperationException("MDT stream is not writable.");

            this.outputStream = outputStream;
            this.tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public void EncodeTokens()
        {
            ushort messageCount = GetMessageCountToken();
            ushort dummy = 0;

            Dictionary<ushort, GSLabelToken> labelsByIndex = GetLabelTokens();
            List<List<IGSToken>> messages = SplitMessages();

            int expectedMessageEntryCount = messageCount - labelsByIndex.Count;
            if (messages.Count != expectedMessageEntryCount)
            {
                throw new InvalidDataException(
                    $"Token stream contains {messages.Count} messages, but header implies {expectedMessageEntryCount} " +
                    $"message entries ({messageCount} total entries, {labelsByIndex.Count} labels).");
            }

            using var writer = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true);

            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.BaseStream.SetLength(0);

            writer.Write(messageCount);
            writer.Write(dummy);

            long offsetTablePos = writer.BaseStream.Position;

            for (int i = 0; i < messageCount; i++)
                writer.Write(0u);

            uint fileMessageOffset = checked((uint)(4 + messageCount * 4));

            List<uint> messageFileOffsets = new List<uint>(messages.Count);
            foreach (List<IGSToken> messageTokens in messages)
            {
                uint absoluteOffset = checked((uint)writer.BaseStream.Position);
                messageFileOffsets.Add(absoluteOffset);

                WriteMessage(writer, messageTokens);
            }

            uint[] table = BuildOffsetTable(messageCount, labelsByIndex, messageFileOffsets);

            long endPos = writer.BaseStream.Position;
            writer.BaseStream.Seek(offsetTablePos, SeekOrigin.Begin);

            foreach (uint entry in table)
                writer.Write(entry);

            writer.BaseStream.Seek(endPos, SeekOrigin.Begin);
        }

        private ushort GetMessageCountToken()
        {
            if (tokens.Count == 0)
                throw new InvalidDataException("First token must be GSMessageCountToken.");

            if (!(tokens[0] is GSMessageCountToken))
                throw new InvalidDataException("First token must be GSMessageCountToken.");

            GSMessageCountToken countToken = (GSMessageCountToken)tokens[0];
            return countToken.MessageCount;
        }

        private Dictionary<ushort, GSLabelToken> GetLabelTokens()
        {
            var result = new Dictionary<ushort, GSLabelToken>();

            foreach (IGSToken token in tokens)
            {
                if (token is GSLabelToken label)
                {
                    if (result.ContainsKey(label.EntryIndex))
                    {
                        throw new InvalidDataException(
                            $"Duplicate GSLabelToken for table entry index {label.EntryIndex}.");
                    }

                    result.Add(label.EntryIndex, label);
                }
            }

            return result;
        }

        private List<List<IGSToken>> SplitMessages()
        {
            var messages = new List<List<IGSToken>>();
            var current = new List<IGSToken>();

            foreach (IGSToken token in tokens.Skip(1)) // skip GSMessageCountToken
            {
                switch (token)
                {
                    case GSEndMessageToken endMessageToken:
                        messages.Add(current);
                        current = new List<IGSToken>();
                        break;

                    default:
                        current.Add(token);
                        break;
                }
            }

            if (current.Count > 0)
            {
                throw new InvalidDataException(
                    "Token stream ended without a final GSEndMessageToken.");
            }

            return messages;
        }

        private uint[] BuildOffsetTable(
            ushort messageCount,
            Dictionary<ushort, GSLabelToken> labelsByIndex,
            List<uint> messageFileOffsets)
        {
            var table = new uint[messageCount];
            int nextMessageOffsetIndex = 0;

            for (ushort entryIndex = 0; entryIndex < messageCount; entryIndex++)
            {
                if (labelsByIndex.TryGetValue(entryIndex, out GSLabelToken? label))
                {
                    table[entryIndex] = PackLabel(label.TargetMessageIndex, label.ByteOffsetWithinMessage);
                    continue;
                }

                if (nextMessageOffsetIndex >= messageFileOffsets.Count)
                {
                    throw new InvalidDataException(
                        $"Not enough message offsets to fill table entry {entryIndex}.");
                }

                table[entryIndex] = messageFileOffsets[nextMessageOffsetIndex++];
            }

            if (nextMessageOffsetIndex != messageFileOffsets.Count)
            {
                throw new InvalidDataException(
                    $"Unused message offsets remain: wrote {nextMessageOffsetIndex}, have {messageFileOffsets.Count}.");
            }

            return table;
        }

        private static uint PackLabel(ushort targetMessageIndex, ushort byteOffsetWithinMessage)
        {
            if ((byteOffsetWithinMessage & 1) != 0)
            {
                throw new InvalidDataException(
                    $"Label byte offset must be 2-byte aligned, got 0x{byteOffsetWithinMessage:X4}.");
            }

            return ((uint)targetMessageIndex << 16) | byteOffsetWithinMessage;
        }

        private void WriteMessage(BinaryWriter writer, List<IGSToken> messageTokens)
        {
            foreach (IGSToken token in messageTokens)
            {
                switch (token)
                {
                    case GSStringToken stringToken:
                        WriteString(writer, stringToken);
                        break;

                    case GSOperationToken operationToken:
                        WriteOperation(writer, operationToken);
                        break;

                    default:
                        throw new InvalidDataException(
                            $"Unsupported token in message stream: {token.GetType().Name}");
                }
            }
        }

        private void WriteString(BinaryWriter writer, GSStringToken stringToken)
        {
            string fullWidth = ToFullWidth(stringToken.Content);

            foreach (char ch in fullWidth)
            {
                ushort encoded = checked((ushort)(ch + 128));
                writer.Write(encoded);
            }
        }

        private static void WriteOperation(BinaryWriter writer, GSOperationToken operationToken)
        {
            writer.Write(operationToken.Opcode);

            foreach (ushort arg in operationToken.Args)
                writer.Write(arg);
        }

        private static string ToFullWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var sb = new StringBuilder(text.Length);

            foreach (char ch in text)
            {
                string s = ch.ToString();
                sb.Append(HalfToFullMap.TryGetValue(s, out string mapped) ? mapped : s);
            }

            return sb.ToString();
        }
    }
}
