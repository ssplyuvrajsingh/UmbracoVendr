using Providers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace USNWebsite.USNControllers
{
    public class RequestedUsersListController : SurfaceController
    {
        private readonly IMemberService _memberService;
        private readonly IRemoveProviderService _removeProviderService;
        private readonly IDummyDataSurfaceServices _dummyDataSurfaceServices;

        public RequestedUsersListController(IMemberService memberService, IRemoveProviderService removeProviderService, IDummyDataSurfaceServices dummyDataSurfaceServices)
        {
            _memberService = memberService;
            _removeProviderService = removeProviderService;
            _dummyDataSurfaceServices = dummyDataSurfaceServices;
        }

        public ActionResult GetAllRequestedUsers(string memberType)
        {
            var members = _memberService.GetAllMembers();
            var result = new List<UsersListResultModel>();
            if (members?.Count() > 0)
            {
                if (memberType == "waitingForApprovalUsers")
                {
                    result = members.Where(x => x.GetValue<string>("providerStatus") == "Waiting for Approval")
                        .Select(x => new UsersListResultModel
                        {
                            Email = x.Email,
                            Name = x.Name,
                            Id = x.Id,
                            ProviderType = x.GetValue<string>("providerType"),
                            Key = x.Key.ToString(),
                            MediaUDI = x.GetValue<string>("iDDocument")
                        }).ToList();
                }
                else if (memberType == "approvedUsers")
                {
                    result = members.Where(x => x.GetValue<string>("providerStatus") == "Approved")
                        .Select(x => new UsersListResultModel
                        {
                            Email = x.Email,
                            Name = x.Name,
                            Id = x.Id,
                            ProviderType = x.GetValue<string>("providerType"),
                            Key = x.Key.ToString(),
                            MediaUDI = x.GetValue<string>("iDDocument")
                        }).ToList();
                }
                else if (memberType == "rejectedUsers")
                {
                    result = members.Where(x => x.GetValue<string>("providerStatus") == "Rejected")
                        .Select(x => new UsersListResultModel
                        {
                            Email = x.Email,
                            Name = x.Name,
                            Id = x.Id,
                            ProviderType = x.GetValue<string>("providerType"),
                            Key = x.Key.ToString(),
                            MediaUDI = x.GetValue<string>("iDDocument")
                        }).ToList();
                }
                else
                {
                    result = members.Select(x => new UsersListResultModel
                    {
                        Email = x.Email,
                        Name = x.Name,
                        Id = x.Id,
                        Key = x.Key.ToString()
                    }).ToList();
                }

                if (result?.Count > 0 && result.Where(x => !string.IsNullOrWhiteSpace(x.MediaUDI)).Count() > 0)
                {
                    foreach (var user in result.Where(x => !string.IsNullOrWhiteSpace(x.MediaUDI)))
                    {
                        var mediaItem = Umbraco.Media(user.MediaUDI);
                        user.FileUrl = mediaItem?.Url();
                        user.FileName = mediaItem?.Name;
                        user.FileType = mediaItem?.GetProperty("umbracoExtension")?.Value<string>();
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string UpdateProviderStatus(string memberId, string status)
        {
            var member = _memberService.GetById(int.Parse(memberId));
            try
            {
                member.SetValue("providerStatus", status);

                if (SaveMember(member))
                {
                    return "Success";
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(RequestedUsersListController), "Failed to update provider status.", ex);
                return "Error";
            }
        }

        [HttpPost]
        public string DeleteProvider(string memberId)
        {
            try
            {
                _removeProviderService.RemoveProvider(Convert.ToInt32(memberId));
                return "Success";
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(RequestedUsersListController), "Failed to delete provider.", ex);
                return "Error";
            }
        }

        
        public string CreateDummyData()
        {
            try
            {
                _dummyDataSurfaceServices.CreateDummyData();
                return "Success";
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(RequestedUsersListController), "Failed to delete provider.", ex);
                return "Error";
            }
        }

        public int MembersUser()
        {
           return _memberService.GetAllMembers().Count();
        }

        protected bool SaveMember(IMember member)
        {
            try
            {
                _memberService.Save(member);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(RequestedUsersListController), "Failed to save member. Member email: " + member.Email, ex);
            }

            return false;
        }
    }
}