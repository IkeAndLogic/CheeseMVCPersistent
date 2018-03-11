using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {


        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            this.context = dbContext;
        }





        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Menu> cheesemenu = context.Menu.ToList();

            return View(cheesemenu);
        }


        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {

            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu()
                {
                    Name = addMenuViewModel.Name
                };

                context.Menu.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }

            return View(addMenuViewModel);
        }



        public IActionResult ViewMenu(int id)
        {

            List<CheeseMenu> items = context.CheesesMenu.Include(item => item.Cheese).Where(cm => cm.MenuID == id).ToList();


            Menu aMenu = context.Menu.Single(m => m.ID == id);

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel()
            {
                Menu = aMenu,
                Items = items


            };

            return View(viewMenuViewModel);
        }




        public IActionResult AddItem(int id)
        {
            Menu aMenu = context.Menu.Single(m => m.ID == id);
            List<Cheese> cheeses = context.Cheeses.ToList();
            AddMenuItemViewModel viewModel = new AddMenuItemViewModel(aMenu, cheeses);
            return View(viewModel);

        }
        
        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {

            if (ModelState.IsValid)
            {
                var cheeseId = addMenuItemViewModel.CheeseID;
                var menuId = addMenuItemViewModel.MenuID;
                IList<CheeseMenu> existingItems = context.CheesesMenu.Where(cm => cm.CheeseID == cheeseId).Where(cm => cm.MenuID == menuId).ToList();

                if (existingItems.Count == 0)
                {
                    CheeseMenu menuItem = new CheeseMenu
                    {
                        CheeseID = cheeseId,
                        MenuID = menuId
                    };

                    context.CheesesMenu.Add(menuItem);
                    context.SaveChanges();
                    return Redirect(string.Format("/Menu/ViewMenu/{0}", addMenuItemViewModel.MenuID));
                }

            }
                return View(addMenuItemViewModel);

        }

    }
}
