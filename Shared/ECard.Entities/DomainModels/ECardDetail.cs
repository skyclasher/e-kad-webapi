using System.ComponentModel.DataAnnotations.Schema;

namespace ECard.Entities.Entities
{
    public class ECardDetail : BaseEntity<int>
    {

        [ForeignKey("User")]
        public int Id_User { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DomainName { get; set; }
    }
}
