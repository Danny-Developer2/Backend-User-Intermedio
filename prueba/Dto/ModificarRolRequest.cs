namespace prueba.Dto
{
    public class ModificarRolRequest
    {
        public Guid Id { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}