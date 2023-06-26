using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP32_Android_Controller.Models;

public class SimpleCommand
{
    public string Name { get; private set; }
    public string Command { get; private set; }

    public SimpleCommand(string name, string command)
    {
        // validate the command
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
        if (string.IsNullOrEmpty(command)) throw new ArgumentNullException("command");
        this.Name = name;
        this.Command = command;

    }
    // what a listview will show
    public override string ToString()
    {
        return this.Name;
    }

}
