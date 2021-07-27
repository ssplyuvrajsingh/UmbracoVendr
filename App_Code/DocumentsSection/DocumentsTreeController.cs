using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http.ModelBinding;
using Umbraco.Core;
using Umbraco.Web.Actions;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;

namespace USNWebsite.USNControllers
{
    [Tree("documentsSection", "documentsSectionTree", TreeTitle = "Document Section", TreeGroup = "documentsGroup", SortOrder = 5)]
    [PluginController("documentsSection")]
    public class DocumentsTreeController : TreeController
    {
        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions, perhaps users can create new items in this tree, or perhaps it's not a content tree, it might be a read only tree, or each node item might represent something entirely different...
                // add your menu item actions or custom ActionMenuItems
                menu.Items.Add(new CreateChildEntity(Services.TextService));
                // add refresh menu item (note no dialog)
                menu.Items.Add(new RefreshNode(Services.TextService, true));
                return menu;
            }
            // add a delete action to each individual item
            menu.Items.Add<ActionDelete>(Services.TextService, true, opensDialog: true);

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            if (id == Constants.System.Root.ToInvariantString())
            {
                Dictionary<int, string> favouriteThings = new Dictionary<int, string>();
                favouriteThings.Add(1, "Documents");
                favouriteThings.Add(2, "Products");
                var nodes = new TreeNodeCollection();

                //foreach (var thing in favouriteThings)
                //{
                //    var node = CreateTreeNode(thing.Key.ToString(), "-1", queryStrings, thing.Value, "icon-presentation", true);
                //    node.RoutePath = "documentsSection";
                //    nodes.Add(node);
                //}

                for (int i = 1; i <= 2; i++)
                {
                    if(i != 2)
                    {
                        var node = CreateTreeNode("1", "-1", queryStrings, "Documents", "icon-presentation", true);
                        node.RoutePath = "documentsSection";
                        nodes.Add(node);
                    }
                    else if(i == 2)
                    {
                        var node = CreateTreeNode("2", "-1", queryStrings, "Products", "icon-presentation", "documentsSection/documentsSectionTree/productsList");
                        nodes.Add(node);
                    }                    
                }
                return nodes;
            }
            else if (id == "1")
            {
                List<DocumentsListModel> favouriteThings = new List<DocumentsListModel>();
                favouriteThings.Add(new DocumentsListModel
                {
                    DocumentPageId = 1,
                    DocumentType = "Waiting For Approval",
                    DocumentRoutePath = "documentsSection/documentsSectionTree/waitingForApprovalUsers",
                });

                favouriteThings.Add(new DocumentsListModel
                {
                    DocumentPageId = 2,
                    DocumentType = "Approved",
                    DocumentRoutePath = "documentsSection/documentsSectionTree/approvedUsers"
                });

                favouriteThings.Add(new DocumentsListModel
                {
                    DocumentPageId = 3,
                    DocumentType = "Rejected",
                    DocumentRoutePath = "documentsSection/documentsSectionTree/rejectedUsers"
                });

                favouriteThings.Add(new DocumentsListModel
                {
                    DocumentPageId = 4,
                    DocumentType = "Payment Withdrawal Requests",
                    DocumentRoutePath = "documentsSection/documentsSectionTree/providerWithdrawalRequests"
                });

                favouriteThings.Add(new DocumentsListModel
                {
                    DocumentPageId = 5,
                    DocumentType = "Payment History",
                    DocumentRoutePath = "documentsSection/documentsSectionTree/paymentHistory"
                });
                var nodes = new TreeNodeCollection();

                foreach (var thing in favouriteThings)
                {
                    var node = CreateTreeNode(thing.DocumentPageId.ToString(), "1", queryStrings, thing.DocumentType, "icon-presentation", thing.DocumentRoutePath);
                    //var node = CreateTreeNode(thing.Key.ToString(), "1", queryStrings, thing.Value, "icon-presentation", false);
                    nodes.Add(node);
                }
                return nodes;
            }
            else if (id == "2")
            {
                Dictionary<int, string> favouriteThings = new Dictionary<int, string>();
                favouriteThings.Add(1, "Product 1");
                favouriteThings.Add(2, "Product 2");
                favouriteThings.Add(3, "Product 3");
                // create our node collection
                var nodes = new TreeNodeCollection();

                // loop through our favourite things and create a tree item for each one
                foreach (var thing in favouriteThings)
                {
                    // add each node to the tree collection using the base CreateTreeNode method
                    // it has several overloads, using here unique Id of tree item, -1 is the Id of the parent node to create, eg the root of this tree is -1 by convention - the querystring collection passed into this route - the name of the tree node -  css class of icon to display for the node - and whether the item has child nodes
                    var node = CreateTreeNode(thing.Key.ToString(), "2", queryStrings, thing.Value, "icon-presentation", false);
                    nodes.Add(node);
                }
                return nodes;
            }

            // this tree doesn't support rendering more than 1 level
            throw new NotSupportedException();
        }
    }
}