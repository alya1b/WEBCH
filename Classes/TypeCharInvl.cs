using System;
using WebApplication.Classes;

namespace WebApplication.Classes
{
    public class TypeCharInvl : Column
    {
        public TypeCharInvl(string name) : base(name)
        {
            Type = TypeColumn.CHARINVL.ToString();
        }

        public override bool Validate(string value)
        {
            string[] buf = value.Replace(" ", "").Split(',');
            return buf.Length == 2 && buf[0].Length == 1 && buf[1].Length == 1 && char.TryParse(buf[0], out char a) &&
            char.TryParse(buf[1], out char b) && a < b;
        }
    }
}
