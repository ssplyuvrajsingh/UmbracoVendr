using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    public class UmbracoSetups
    {
        public string ConfigJson(string status)
        {
            var jsonFilePath = string.Empty;
            switch (status)
            {
                case "providerType":
                    jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Code/DatabaseRepo/DatabaseMigration/ConfigJson/ProviderType.json");
                    break;
                case "providerStatus":
                    jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Code/DatabaseRepo/DatabaseMigration/ConfigJson/ProviderStatus.json");
                    break;
                case "usnBlockComponents":
                    jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Code/DatabaseRepo/DatabaseMigration/ConfigJson/UsnBlockComponents.json");
                    break;
                case "optionFormType":
                    jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Code/DatabaseRepo/DatabaseMigration/ConfigJson/OptionFormType.json");
                    break;
            }

            string json = string.Empty;
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                json = r.ReadToEnd();
            }
            return json;
        }
        public string MemberPropertyAdd()
        {
            var query = $@"Insert cmsPropertyTypeGroup(contenttypeNodeId,text,sortorder,uniqueID) 
                            values(1044,'Provider Details',0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}')) 

                            declare @ptGroupId int = SCOPE_IDENTITY()

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'firstName', 'First Name', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}')) 

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'lastName', 'Last Name', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}')) 

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'streetName', 'Street Name', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'city', 'City', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'zipCode', 'Zip Code', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'country', 'Country', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'telephoneNumber', 'Telephone Number', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                            values(-88,1044,@ptGroupId,'emailAddress', 'Email Address', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                             values(1048,1044,@ptGroupId,'iDDocument', 'ID Document', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))


                            Insert umbracoNode
                            (uniqueId, parentId, level,path,sortOrder,trashed,nodeUser,text,nodeObjectType,createDate)
                            values(Convert(uniqueidentifier, '1CBBD878-B72E-48A1-AE34-EBBB2FF59242'), -1, 1, -1,30,0,-1,
                            'Member - Provider Type - Radio button list', '30A2A501-1978-4DDB-A57B-F7EFED43BA3C',GETDATE());

                            declare @uNodeId int = SCOPE_IDENTITY()

                            update umbracoNode set path = '-1' + CONVERT(varchar,@uNodeId) where id = @uNodeId

                            Insert umbracoDataType(nodeId,propertyEditorAlias,dbType,config) 
                            values(@uNodeId,'Umbraco.RadioButtonList','Nvarchar','{ConfigJson("providerType")}')

                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                             values(@uNodeId,1044,@ptGroupId,'providerType', 'Provider Type', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))

                            Insert umbracoNode
                                (uniqueId, parentId, level,path,sortOrder,trashed,nodeUser,text,nodeObjectType,createDate)
                                values(Convert(uniqueidentifier, '423E509C-9585-4D19-B9C3-7D6FDBA6813B'), -1, 1, -1,31,0,-1,
                                'Provider Status - Radio button list', '30A2A501-1978-4DDB-A57B-F7EFED43BA3C',GETDATE());

                                 set @uNodeId = SCOPE_IDENTITY()

                                update umbracoNode set path = '-1' + CONVERT(varchar,@uNodeId) where id = @uNodeId

                            Insert umbracoDataType(nodeId,propertyEditorAlias,dbType,config) 
                                values(@uNodeId,'Umbraco.RadioButtonList','Nvarchar','{ConfigJson("providerStatus")}')
                            
                            Insert cmsPropertyType(dataTypeId,contentTypeId,propertyTypeGroupId,Alias,Name,sortOrder,mandatory,labelOnTop,variations,UniqueID)
                             values(@uNodeId,1044,@ptGroupId,'providerStatus', 'Provider Status', 0,0,0,0,CONVERT(uniqueidentifier,'{Guid.NewGuid()}'))
                        ";
            return query.Replace("@", "@@");
        }

        public string GetUsnBlockComponents()
        {
            return $@"Select config from  umbracoDataType where nodeId = (select id from umbracoNode  where uniqueId = '09B761E4-10A2-498B-BC3E-D7755C9EE8AD');";
        }
        public string UsnBlockComponents(string config)
        {
            var configModel =  JsonConvert.DeserializeObject<UsnBlockComponentsConfig.Root>(config);

            var blocks = JsonConvert.DeserializeObject<UsnBlockComponentsConfig.Root>(ConfigJson("usnBlockComponents"));

            configModel.blocks.AddRange(blocks.blocks);

            var json = JsonConvert.SerializeObject(configModel).Replace("'", "''");

            var query = $@"
                        declare @Id int;
                        select @id=id from umbracoNode  where uniqueId = '09B761E4-10A2-498B-BC3E-D7755C9EE8AD';
                        select @Id;
                        Update umbracoDataType set config = '{json}' where nodeId = @Id;
                        ";
            return query.Replace("@", "@@");
        }
        public string GetOptionFormType()
        {
            return $@"Select config from umbracoDataType where nodeId = (select id from umbracoNode  where uniqueId = '789c570a-726e-4c0d-92a1-dcd47a14961f');";
        }
        public string OptionFormType(string config)
        {
            var configModel = JsonConvert.DeserializeObject<OptionFormTypeConfig.Root>(config);

            var item = JsonConvert.DeserializeObject<OptionFormTypeConfig.Root>(ConfigJson("optionFormType"));
            configModel.items.items.AddRange(item.items.items);

            var json = JsonConvert.SerializeObject(configModel).Replace("'", "''");

            var query = $@"
                        declare @Id int;
                        select @id=id from umbracoNode  where uniqueId = '789c570a-726e-4c0d-92a1-dcd47a14961f';
                        select @Id;
                        Update umbracoDataType set config = '{json}' where nodeId = @Id;
                        ";
            return query.Replace("@", "@@");
        }
    }
}