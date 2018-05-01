using System.Collections.Generic;
using System.IO;

namespace GRT
{
    public class FileParser
    {
        protected bool fileParsed;
        protected bool consistentColumnSize;
        protected uint columnSize;
        protected Vector<Vector<string>> fileContents;

        public FileParser()
        {
            fileContents = new Vector<Vector<string>>();
            Clear();
        }

        ~FileParser()
        {
            Clear();
        }

        public Vector<string> this[int index]
        {
            get { return fileContents[index]; }
        }

        public bool ParseCSVFile(string filename, bool removeNewLineCharacter = true)
        {
            return ParseFile(filename, removeNewLineCharacter, ',');
        }

        public bool ParseTSVFile(string filename, bool removeNewLineCharacter = true)
        {
            return ParseFile(filename, removeNewLineCharacter, '\t');
        }

        public bool FileParsed => fileParsed;
        public bool ConsistantColmunSize => consistentColumnSize;
        public uint RowSize => (uint)fileContents.Count;
        public uint ColumnSize => columnSize;
        public Vector<Vector<string>> Filecontents => fileContents;

        public bool Clear()
        {
            fileParsed = false;
            consistentColumnSize = false;
            columnSize = 0;
            fileContents.Clear();
            return true;
        }

        protected static bool ParseColumn(string row, Vector<string> cols, char seperator = ',')
        {
            uint N = (uint)row.Length;
            if (N == 0) { return false; }

            int lastSize = cols.Count; ;
            cols.Clear();

            if (lastSize > 0) { cols.Capacity = lastSize; } //Reserve the previous column size
            string columnString = "";
            int sepValue = seperator;
            for (int i = 0; i < N; i++)
            {
                if ((int)row[i] == sepValue)
                {
                    cols.Add(columnString);
                    columnString = "";
                }
                else columnString += row[i];
            }

            //Add the last column
            cols.Add(columnString);

            //Remove the new line char from the string in the last column
            if (cols.Count >= 1)
            {
                int K = cols.Count - 1;
                int foundA = cols[K].IndexOf('\n');
                int foundB = cols[K].IndexOf('\r');
                if (foundA < 0 || foundB < 0)
                {
                    cols[K] = cols[K].Substring(0, cols[K].Length - 1);
                }
            }
            return true;
        }

        protected bool ParseFile(string filename, bool removeNewLineCharacter, char seperator)
        {
            //Clear any previous data
            Clear();

            bool success = false;

            using (StreamReader sr = new StreamReader(filename))
            {
                var vec = new Vector<string>();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!ParseColumn(line, vec, seperator))
                    {
                        Clear();
                        Log.Warning("parseFile(...) - Failed to parse column!");
                        break;
                    }

                    //Check to make sure all the columns are consistent
                    if (columnSize == 0)
                    {
                        consistentColumnSize = true;
                        columnSize = vec.GetSize();
                    }
                    else if (columnSize != vec.GetSize())
                    {
                        consistentColumnSize = false;
                    }
                    fileContents.Add(vec);
                }
                success = true;
            }

            return success;
        }
    }
}
