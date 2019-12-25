using ECard.Business.Generics;
using ECard.Data.Infrastructure;
using ECard.Entities.Entities;
using Microsoft.Extensions.Options;
using Project.Framework.Configuration;
using Project.Framework.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ECard.Business.XloRecords
{
	public class XloRecordComponent : GenericComponent<XloRecord>, IXloRecordComponent
	{
		private ECardDataContext _context;
		private readonly AppSettings _appSettings;

		public XloRecordComponent(ECardDataContext context, IOptions<AppSettings> appSettings) : base(context, appSettings)
		{
			_context = context;
			_appSettings = appSettings.Value;
		}

		public PagingHelper<XloRecord> GetPagedXloRecordByCardDetailId(int cardDetailId, int currentPage, string searchText)
		{
			int ecardId = _context.XloRecord.Where(x => x.Id_EcardDetail == cardDetailId).SingleOrDefault().Id;

			Expression<Func<XloRecord, bool>> filter =
				   x => (x.Id_EcardDetail == ecardId)
						&&
						(string.IsNullOrEmpty(searchText) ? true :
						   (x.Name.ToLower().Contains(searchText.ToLower()) ||
						   x.AttCount.ToString().Contains(searchText))
						);


			var data = _context.XloRecord.Where(filter).ToList();

			return PagingXloRecordByCardDetailId(data, searchText, currentPage);
		}


		private PagingHelper<XloRecord> PagingXloRecordByCardDetailId(List<XloRecord> data, string searchText, int currentPage)
		{
			return PagingHelper<XloRecord>.ToPaging(data, x => x.OrderBy(y => y.Name), searchText, _appSettings.PageSize, currentPage);
		}
	}
}
