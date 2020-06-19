﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.MangerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            //It will loads to category details of subcategory
            var subCategory = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategory);
        }

        //Get Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory
                                               .Include(s => s.Category)
                                               .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : Sub Category existing under " + doesSubCategoryExists.First().Category.Name + "category. Please use another Name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name)
                                                   .Select(p => p.Name)
                                                   .ToListAsync(),
                StatusMessage = StatusMessage

            };

            return View(modelVm);
        }


        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id) {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //Get EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null) {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        //Post EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory
                                               .Include(s => s.Category)
                                               .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : Sub Category existing under " + doesSubCategoryExists.First().Category.Name + "category. Please use another Name.";
                }
                else
                {
                    var subCatFromDB = await _db.SubCategory.FindAsync(id);
                    subCatFromDB.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name)
                                                   .Select(p => p.Name)
                                                   .ToListAsync(),
                StatusMessage = StatusMessage

            };
            //modelVm.SubCategory.Id = id; OR add hidden filed in html then id not required here as parameter.
            return View(modelVm);
        }

        //Get Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }            
            return View(subCategory);
        }

        //Get Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }           
            return View(subCategory);
        }

        //POST Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, SubCategoryAndCategoryViewModel model)
        {
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
