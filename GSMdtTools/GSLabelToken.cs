namespace GSMdtTools
{
    class GSLabelToken : IGSToken
    {
        public ushort EntryIndex { get; }
        public ushort TargetMessageIndex { get; }
        public ushort ByteOffsetWithinMessage { get; }

        public GSLabelToken(ushort entryIndex, ushort targetMessageIndex, ushort byteOffsetWithinMessage)
        {
            EntryIndex = entryIndex;
            TargetMessageIndex = targetMessageIndex;
            ByteOffsetWithinMessage = byteOffsetWithinMessage;
        }

        public string Type()
        {
            return "GSLabelToken";
        }
    }
}
