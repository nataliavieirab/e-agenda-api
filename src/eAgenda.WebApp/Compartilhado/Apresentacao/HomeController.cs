using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Compartilhado.Apresentacao;

public class HomeController : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }
}
