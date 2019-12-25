using ECard.Business.Generics;
using ECard.Entities.Entities;
using Project.Framework.Helper;

namespace ECard.Business.XloRecords
{
	public interface IXloRecordComponent : IGenericComponent<XloRecord>
	{
		PagingHelper<XloRecord> GetPagedXloRecordByCardDetailId(int cardDetailId, int currentPage, string searchText);
	}
}
