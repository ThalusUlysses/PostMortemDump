namespace Thalus.Ulysses.PostMortem.Dump
{

    /// <summary>
    /// Implememts the post morte write to file
    /// </summary>
    public class PostMortemFileWriter
    {
        string _location;

        /// <summary>
        /// Creates an instance of <see cref="PostMortemFileWriter"/> with the passed parameters
        /// </summary>
        /// <param name="location"></param>
        public PostMortemFileWriter(string location)
        {
            _location = location;
        }

        /// <summary>
        /// Writes a json representation of the passed object
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>Please note every exception will be catched due to the process is terminating anyways</remarks>
        public void Write(object data)
        {
            try
            {
                if (!Directory.Exists(_location))
                {
                    Directory.CreateDirectory(_location);
                }

                string fileName = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json";
                string path = Path.Combine(_location, fileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.WriteAllText(path, data.ToJson(true));
            }
            catch (Exception)
            {

                // okay to do so as it will be just in post mortem state
            }

        }
    }
}
