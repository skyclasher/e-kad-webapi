using System.ComponentModel.DataAnnotations.Schema;

namespace ECard.Entities.Entities
{
    public class Rsvp : BaseEntity<int>
    {

        [ForeignKey("ECardDetail")]
        public int Id_EcardDetail { get; set; }
        public ECardDetail ECardDetail { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Attendance { get; set; }
        public int AttCount { get; set; }
        public string Wishes { get; set; }
        public string TelNo { get; set; }

    }
}
