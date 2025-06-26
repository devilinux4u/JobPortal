using JobPortal.Models;

public class JobListViewModel
{
    public List<Job> Jobs { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}