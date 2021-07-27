using System;
using System.Web.Mvc;
using System.Web.Security;
using USNWebsite.USNModels;
using Umbraco.Core.Services;
using System.Configuration;
using Umbraco.Core.Models;
using Providers.Services;

public class MemberSurfaceController : Umbraco.Web.Mvc.SurfaceController
{
    private readonly IMemberService _memberService;
    private readonly IGeneralService _generalService;

    public MemberSurfaceController(IMemberService memberService, IGeneralService generalService)
    {
        _memberService = memberService;
        _generalService = generalService;
    }
    [HttpPost]
    public void HandleRegisterSubmit(ProviderRootModel rootModel)
    {
        try
        {
            foreach (var item in rootModel.ProvidersModel)
            {
                rootModel.RegisterModel = Members.CreateRegistrationModel();
                rootModel.RegisterModel.Email = item.Email;
                rootModel.RegisterModel.Name = item.FirstName + " "+ item.LastName;
                rootModel.RegisterModel.Password = item.Password;

                string username = rootModel.RegisterModel.Email;
                string memberTypeAlias = string.IsNullOrEmpty(ConfigurationManager.AppSettings["USN_DefaultMemberType"]) ? "Member" : ConfigurationManager.AppSettings["USN_DefaultMemberType"];
                string memberGroupAlias = string.IsNullOrEmpty(ConfigurationManager.AppSettings["USN_DefaultMemberGroup"]) ? "Main Client" : ConfigurationManager.AppSettings["USN_DefaultMemberGroup"];

                MembershipCreateStatus membershipCreateStatus;
                rootModel.RegisterModel.Name = string.IsNullOrEmpty(rootModel.RegisterModel.Name) ? rootModel.RegisterModel.Email : rootModel.RegisterModel.Name;
                MembershipUser membershipUser = base.Members.RegisterMember(rootModel.RegisterModel, out membershipCreateStatus, false);
                IMemberService memberService = Services.MemberService;

                if (MembershipCreateStatus.Success == 0)
                {
                    var user = memberService.GetByUsername(membershipUser.UserName);
                    user.IsApproved = true;
                    memberService.AssignRole(membershipUser.UserName, memberGroupAlias);
                    memberService.Save(user);

                    USNProviderRegisterFormViewModel providrModel = new USNProviderRegisterFormViewModel
                    {
                        Id = user.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        ProviderType = item.ProviderType,
                        StreetName = item.StreetName,
                        City = item.City,
                        ZipCode = item.ZipCode,
                        Country = item.Country,
                        TelephoneNumber = item.TelephoneNumber,
                        Email = item.Email,
                        ProviderStatus = item.ProviderStatus,
                        IdDocumentPath = item.IdDocument
                    };

                    HandleProviderRegisterSubmit(providrModel);
                }
            }
        }
        catch(Exception ex)
        {
        }
    }

    public void HandleProviderRegisterSubmit(USNProviderRegisterFormViewModel model)
    {
        var idDocument = _generalService.GetFileGuid(model.IdDocumentPath.Substring(1));

        var member = _memberService.GetById(model.Id);
        member.SetValue("firstName", model.FirstName);
        member.SetValue("lastName", model.LastName);
        member.SetValue("providerType", model.ProviderType);
        member.SetValue("streetName", model.StreetName);
        member.SetValue("city", model.City);
        member.SetValue("zipCode", model.ZipCode);
        member.SetValue("country", model.Country);
        member.SetValue("telephoneNumber", model.TelephoneNumber);
        member.SetValue("emailAddress", model.Email);
        member.SetValue("iDDocument", idDocument);
        member.SetValue("providerStatus", model.ProviderStatus);
        SaveMember(member);
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
            Logger.Error(typeof(MemberSurfaceController), "Failed to save member. Member email: " + member.Email, ex);
        }

        return false;
    }
}