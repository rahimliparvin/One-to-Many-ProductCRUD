namespace EntityFramework_Slider.Helpers
{
	public class Paginate<T>
	{
		public List<T> Datas { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }

		public Paginate(List<T> datas , int currentPage , int totalPages)
		{
			Datas = datas;
			CurrentPage = currentPage;
			TotalPages = totalPages;
		}

		public bool HasPrevious
		{
			get 
			{
			    return @CurrentPage > 1;
			}
		}

		public bool HasNext
		{
			get
			{
				return @CurrentPage < TotalPages;
			}
		}
	}
}
