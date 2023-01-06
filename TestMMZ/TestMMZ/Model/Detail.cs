namespace TestMMZ.Model
{
    public class Detail
    {
        public int? Id { get; }
        public string Name { get; set; }
        public string Oboz { get; set; }
        public string? Note { get; set; }

        public Detail(int? id, string name, string oboz, string? note)
        {
            Id = id ?? 0;
            Name = name;
            Oboz = oboz;
            Note = note ?? "";
        }
    }
}
