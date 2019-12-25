using System.ComponentModel.DataAnnotations.Schema;

namespace ECard.Entities.Entities
{
	public class XloRecord : BaseEntity<int>
	{

		[ForeignKey("ECardDetail")]
		public int Id_EcardDetail { get; set; }
		public ECardDetail ECardDetail { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string DOB { get; set; }
		public string AttCount { get; set; }
		public string TelNo { get; set; }
		public string OfficeTelNo { get; set; }
		public string Address { get; set; }
		public string LastClass { get; set; }
		public string Work { get; set; }
		public string WedStatus { get; set; }
		public string BilSon { get; set; }

	}
}
