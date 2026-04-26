namespace MiniWordPro
{
    public static class WordCounter
    {
        public static int Count(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Split(' ').Length;
        }
    }
}