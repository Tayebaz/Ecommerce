using AutoMapper;
using Business;
using Entities.Models;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;

namespace GrihastiWebsite.Controllers
{
    public class AboutController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private HelpBusiness _helpBusiness;

        public AboutController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._helpBusiness = new HelpBusiness(_df, this._unitOfWork);

        }
        //
        // GET: /HelpUMart/

        public ActionResult Index(string helpType = "About")
        {
            ViewBag.helpType = helpType;
            return View();
        }

        public ActionResult LoadContent(string helpType)
        {
            var help = _helpBusiness.GetListWT(c => c.ContentType == helpType).FirstOrDefault();
            Mapper.CreateMap<Help, HelpViewModel>();
            var vmHelp = Mapper.Map<Help, HelpViewModel>(help);
            return PartialView("_helpContent", vmHelp);
        }

    }
}
