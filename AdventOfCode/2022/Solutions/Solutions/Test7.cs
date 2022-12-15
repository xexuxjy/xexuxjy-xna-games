using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test7 : BaseTest
{
    public override void RunTest()
    {
        string filename = InputPath + "puzzle-7-input.txt";

        FileSystem fileSystem = new FileSystem();

        string currentCommandLine = null;
        List<string> currentCommandResults = new List<string>();

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    if (line.StartsWith("$"))
                    {
                        if (currentCommandLine != null)
                        {
                            HandleCommand(currentCommandLine, currentCommandResults, fileSystem);
                        }
                        currentCommandResults.Clear();
                        currentCommandLine = line;

                    }
                    else
                    {
                        currentCommandResults.Add(line);
                    }
                    int ibreak = 0;
                }
            }
            // handle last command before EOF
            HandleCommand(currentCommandLine, currentCommandResults, fileSystem);

        }


        int hereIsMyIncrediblyLongVariableNameThatIHateTyping = 10;
        hereIsMyIncrediblyLongVariableNameThatIHateTyping += 1;

        int size = 100000;
        List<FileSystemNode> results = fileSystem.FindDirectoriesLessThanSize(size);
        int total = 0;
        foreach (FileSystemNode node in results)
        {
            total += node.Size;
        }

        FileSystemNode result = null;
        int smallest = int.MaxValue;
        fileSystem.FindSmallestToDelete(30000000,ref smallest,fileSystem.RootDirectory,ref result);

        int ibreak2 = 0;

    }

    static string DIRECTORY = "dir";

    private static void HandleCommand(string line, List<string> results, FileSystem fileSystem)
    {
        Debug.Assert(line.StartsWith("$ "));
        line = line.Remove(0, 2);
        string[] tokens = line.Split(" ");
        if (tokens[0] == "cd")
        {
            string targetDirectory = tokens[1];
            if (targetDirectory == "/")
            {
                fileSystem.ChangeToRoot();
            }
            else if (targetDirectory == "..")
            {
                fileSystem.ChangeUp();
            }
            else
            {
                fileSystem.ChangeDown(tokens[1]);
            }
        }
        else if (tokens[0] == "ls")
        {
            foreach (string result in results)
            {
                bool isDirectory = false;
                int fileSize = 0;

                string[] resultTokens = result.Split(" ");
                if (resultTokens[0] == DIRECTORY)
                {
                    isDirectory = true;
                }
                else if (int.TryParse(resultTokens[0], out fileSize))
                {

                }

                fileSystem.AddChild(resultTokens[1], isDirectory, fileSize);
                //List<string> results = fileSystem.ListFiles();
            }
        }

    }
}
public class FileSystem
{
    private int m_maxSpace = 0;
    private FileSystemNode m_root;

    public FileSystem(int maxSpace = 70000000)
    {
        m_root = new FileSystemNode("Root", true, 0);
        CurrentDirectory = RootDirectory;
        m_maxSpace = maxSpace;
    }

    public int AvailableSpace
    {
        get { return m_maxSpace - RootDirectory.Size; }
    }




    public FileSystemNode CurrentDirectory
    {
        get; set;
    }

    public FileSystemNode RootDirectory
    {
        get { return m_root; }
    }



    public void ChangeToRoot()
    {
        CurrentDirectory = m_root;
    }

    public void ChangeUp()
    {
        if (CurrentDirectory != RootDirectory)
        {
            CurrentDirectory = CurrentDirectory.Parent;
        }
    }

    public void ChangeDown(string name)
    {
        FileSystemNode child = CurrentDirectory.GetChild(name);
        if (child != null)
        {
            CurrentDirectory = child;
        }
    }

    public List<string> ListFiles()
    {
        List<string> results = new List<string>();
        foreach (FileSystemNode fsn in CurrentDirectory.Children)
        {
            results.Add(fsn.Name);
        }
        return results;
    }

    public FileSystemNode AddChild(string name, bool isDir, int fileSize)
    {
        FileSystemNode child = CurrentDirectory.GetChild(name);
        if (child == null)
        {
            child = new FileSystemNode(name, isDir, fileSize);
            CurrentDirectory.AddChild(child);
        }
        return child;
    }

    public List<FileSystemNode> FindDirectoriesLessThanSize(int size)
    {
        List<FileSystemNode> results = new List<FileSystemNode>();
        FindDirectoriesLessThanSize(size, RootDirectory, results);
        return results;
    }

    private void FindDirectoriesLessThanSize(int size, FileSystemNode node, List<FileSystemNode> results)
    {
        if (node.IsDirectory && node.Size < size)
        {
            results.Add(node);
        }
        if (node.Children != null)
        {
            foreach (FileSystemNode fsn in node.Children)
            {
                FindDirectoriesLessThanSize(size, fsn, results);
            }
        }
    }

    public void FindSmallestToDelete(int requiredSize, ref int smallestVal, FileSystemNode node, ref FileSystemNode chosenNode)
    {
        if (node.IsDirectory)
        {
            if (AvailableSpace + node.Size > requiredSize && node.Size < smallestVal)
            {
                chosenNode = node;
                smallestVal = node.Size;

            }
            foreach (FileSystemNode childNode in node.Children)
            {
                FindSmallestToDelete(requiredSize, ref smallestVal, childNode, ref chosenNode);
            }
        }
    }

}

public class FileSystemNode
{
    public bool IsDirectory { get; set; }
    public FileSystemNode Parent;

    private List<FileSystemNode> m_children;

    private int m_size;

    public string Name { get; set; }

    public string DisplayName
    {
        get
        {
            string result = IsDirectory ? "dir" : "";
            result += " ";
            result += Name;
            return result;
        }
    }
    public FileSystemNode(string name, bool isDirectory, int size)
    {
        Name = name;
        IsDirectory = isDirectory;
        m_size = size;
    }

    public IEnumerable<FileSystemNode> Children
    {
        get
        {
            return m_children;
        }
    }

    public void AddChild(FileSystemNode node)
    {
        if (m_children == null)
        {
            m_children = new List<FileSystemNode>();
        }
        if (!m_children.Contains(node))
        {
            node.Parent = this;
            m_children.Add(node);
        }
    }

    public FileSystemNode GetChild(string name)
    {
        if (m_children != null)
        {
            foreach (FileSystemNode node in m_children)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }

        }
        return null;
    }
    public int Size
    {
        get
        {
            if (!IsDirectory)
            {
                return m_size;
            }
            else
            {
                int total = 0;
                foreach (FileSystemNode node in m_children)
                {
                    total += node.Size;
                }
                return total;
            }
        }

    }
}
