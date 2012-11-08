
namespace MappingTheInternet.Models
{
    public class ASNode
    {
        public readonly string Name;
        public readonly int? Number;

        public ASNode(string name)
        {
            Name = name;
        }

        public ASNode(int number)
        {
            Number = number;
        }
    }
}
