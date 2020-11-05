namespace ImageRecognition
{
    public class ImageDetails
    {
        public ImageDetails(string name, string fullName, int index)
        {
            Name = name;
            FullName = fullName;
            Index = index;
        }
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public int Index { get; private set; }
    }
}
