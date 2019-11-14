using AutoMapper;
using Business;
using Commom.GlobalMethods;
using Entities.Models;
using Filters.ActionFilters;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Help")]
    public class AdminHelpController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private HelpBusiness _helpBusiness;

        public AdminHelpController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._helpBusiness = new HelpBusiness(_df, this._unitOfWork);

        }
        //
        // GET: /Help/
        [Route("")]
        public ActionResult Index(string helpType = "About")
        {
            ViewBag.helpType = helpType;
            return View();
        }

        [Route("LoadContent")]
        public ActionResult LoadContent(string helpType)
        {
            var help = _helpBusiness.GetListWT(c => c.ContentType == helpType).FirstOrDefault();
            Mapper.CreateMap<Help, HelpViewModel>();
            var vmHelp = Mapper.Map<Help, HelpViewModel>(help);
            return PartialView("_helpContent", vmHelp);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("ChangeContent")]
        public ActionResult ChangeContent(HelpViewModel help)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Mapper.CreateMap<HelpViewModel, Help>();
                    Help content = Mapper.Map<HelpViewModel, Help>(help);

                    var contentdb = _helpBusiness.GetListWT(c => c.ContentType == help.ContentType).FirstOrDefault();
                    if(contentdb == null)
                    {
                        content.TokenKey = GlobalMethods.GetToken();
                        _helpBusiness.Insert(content);
                        _unitOfWork.SaveChanges();                       
                    }
                    else
                    {
                        contentdb.Contents = content.Contents;
                        _helpBusiness.Update(contentdb);
                        _unitOfWork.SaveChanges();                       
                    }
                    TempData["Success"] = "Content changed Successfully!!";
                    TempData["isSuccess"] = "true";
                    return RedirectToAction("Index", new { helpType = help.ContentType });
                }
                catch (Exception ex)
                {
                    TempData["Success"] = "Failed to change Content!!";
                    TempData["isSuccess"] = "false";
                    return RedirectToAction("Index", new { helpType = help.ContentType });
                    throw ex;
                }
            }
            else
            {
                TempData["Success"] = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                TempData["isSuccess"] = "false";
            }
            return RedirectToAction("Index");
        }
    }
}
