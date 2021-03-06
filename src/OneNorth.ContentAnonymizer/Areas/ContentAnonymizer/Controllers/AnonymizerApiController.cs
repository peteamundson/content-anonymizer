﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using OneNorth.ContentAnonymizer.Areas.ContentAnonymizer.Models;
using OneNorth.ContentAnonymizer.Data;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Extensions;

namespace OneNorth.ContentAnonymizer.Areas.ContentAnonymizer.Controllers
{
    [AuthorizeAnonymizerApi]
    public class AnonymizerApiController : Controller
    {
        private readonly ID _mediaFolderTemplateId = new ID("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");
        private readonly ID _templateTemplateId = new ID("{AB86861A-6030-46C5-B394-E8F99E8B87DB}");

        public ActionResult GetTemplates(string filter)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");
            if (index == null)
                throw new ApplicationException("sitecore_master_index not found");

            List<SearchResultItem> results;
            using (var context = index.CreateSearchContext())
            {
                ID directIdMatch;

                if (ID.TryParse(filter, out directIdMatch))
                {
                    // if there is a guid passed into the search filter, then directly search for the item
                    results = context.GetQueryable<SearchResultItem>()
                        .Where(x => x.TemplateId == _templateTemplateId && x.ItemId == directIdMatch)
                        .ToList();
                }
                else
                {
                    // if the filter is just a text item, then search for the value within the name
                    results = context.GetQueryable<SearchResultItem>()
                        .Where(x => x.TemplateId == _templateTemplateId && x[BuiltinFields.LatestVersion].Equals("1") && x.Language.Equals("en"))
                        .ToList()
                        .Where(x => x.Name.Contains(filter) || x.Path.Contains(filter))
                        .ToList();
                }
            }

            var items = results
                    .OrderBy(x => x.Path)
                    .Take(10)
                    .Select(result => new Models.TemplateInfo { Id = result.ItemId.ToGuid(), Path = result.Path })
                    .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFields(string templateId)
        {
            var database = Database.GetDatabase("master");
            if (database == null)
                throw new ApplicationException("master database not found");

            var template = database.GetTemplate(new ID(templateId));

            var fields = template.Fields.Where(x => !x.Name.StartsWith("__"))
                                 .Select(x => new FieldInfo { Anonymize = AnonymizeType.None, DisplayName = x.DisplayName, Id = x.ID.ToGuid(), Name = x.Name, Type = x.Type})
                                 .OrderBy(x => x.DisplayName);

            var content = JsonConvert.SerializeObject(fields);
            return Content(content, "application/json");
        }

        public ActionResult GetStandardFields(string templateId)
        {
            var database = Database.GetDatabase("master");
            if (database == null)
                throw new ApplicationException("master database not found");

            var template = database.GetTemplate(new ID(templateId));

            var fields = template.Fields.Where(x => x.Name.StartsWith("__"))
                                 .Select(x => new FieldInfo { Anonymize = AnonymizeType.None, DisplayName = x.DisplayName, Id = x.ID.ToGuid(), Name = x.Name, Type = x.Type })
                                 .OrderBy(x => x.DisplayName);

            var content = JsonConvert.SerializeObject(fields);
            return Content(content, "application/json");
        }

        public JsonResult GetItems(string templateId)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");
            if (index == null)
                throw new ApplicationException("sitecore_master_index not found");

            var templateIdAsId = new ID(templateId);

            // Select the distinct items based on template.
            List<SearchResultItem> results;
            using (var context = index.CreateSearchContext())
            {
                results = context.GetQueryable<SearchResultItem>()
                    .Where(x => x.TemplateId == templateIdAsId && x.Parent != templateIdAsId && x[BuiltinFields.LatestVersion].Equals("1"))
                    .ToList();
            }

            var items = results
                    .GroupBy(x => x.ItemId)
                    .Select(g => g.First())
                    .Select(result => new ItemInfo { Id = result.ItemId.ToGuid(), Path = result.Path })
                    .OrderBy(x => x.Path)
                    .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguages()
        {
            // Obtain a reference to the master database
            var database = Database.GetDatabase("master");
            if (database == null)
                throw new ApplicationException("master database not found");

            // Retrieve the item from the database, and process if it exists.
            var languages = database.GetLanguages();
            var languageInfos = languages.Select(language => new LanguageInfo { DisplayName = language.GetDisplayName(), Name = language.Name })
                                .OrderBy(x => x.Name)
                                .ToList();

            return Json(languageInfos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMediaFolders(string filter)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");
            if (index == null)
                throw new ApplicationException("sitecore_master_index not found");

            List<SearchResultItem> results;
            using (var context = index.CreateSearchContext())
            {
                results = context.GetQueryable<SearchResultItem>()
                    .Where(x => x.TemplateId == _mediaFolderTemplateId && x[BuiltinFields.LatestVersion].Equals("1") && x.Language.Equals("en"))
                    .ToList()
                    .Where(x => x.Name.Contains(filter) || x.Path.Contains(filter))
                    .ToList();
            }

            var items = results
                .OrderBy(x => x.Path)
                .Take(10)
                .Select(result => new ItemInfo { Id = result.ItemId.ToGuid(), Path = result.Path })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Anonymize(string options)
        {
            // Obtain a reference to the master database
            var database = Database.GetDatabase("master");
            if (database == null)
                throw new ApplicationException("master database not found");

            var anonymizeOptions = JsonConvert.DeserializeObject<AnonymizeOptions>(options);

            // Loop through each item
            foreach (var itemInfo in anonymizeOptions.Items)
            {
                // Decide if we should process the item.
                if (!itemInfo.Anonymize)
                    continue;

                // Retrieve the item from the database, and process if it exists.
                var item = database.GetItem(new ID(itemInfo.Id));
                if (item == null)
                    continue;

                // Determine translations to process
                var translations = item.Languages
                    .Select(x => database.GetItem(item.ID, x))
                    .Where(x => x != null && x.Versions.Count > 0);

                // Process each translation
                foreach(var translation in translations)
                {
                    // Reset the translation back to the version 1
                    ResetItemVersion(translation);

                    // Anonymize the translation
                    ItemAnonymizer.Instance.AnonymizeItem(translation, anonymizeOptions);
                }
            }
        }

        private void ResetItemVersion(Sitecore.Data.Items.Item item)
        {
            item.Versions.RemoveAll(false);

            // We need to use private property assignment to change the version to 1
            // Someone hold my beer...
            if (item.Version.Number > 1)
            {
                var itemDataProp = item.GetType().GetField("_innerData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var newItemData = new ItemData(item.InnerData.Definition, item.InnerData.Language, Sitecore.Data.Version.Parse(1), item.InnerData.Fields);
                itemDataProp.SetValue(item, newItemData);
            }

            // We need to let all the fields think that they have been changed
            // Someone hold my other beer...
            var itemChanges = new Sitecore.Data.Items.ItemChanges(item);
            foreach (Sitecore.Data.Fields.Field field in item.Fields)
            {
                itemChanges.SetFieldValue(field, field.Value, string.Empty);
            }

            var changesDataProp = item.GetType().GetField("_changes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            changesDataProp.SetValue(item, itemChanges);
        }
    }
}