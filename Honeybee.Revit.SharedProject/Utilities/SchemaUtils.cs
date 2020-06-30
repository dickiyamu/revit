using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using NLog;
// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Honeybee.Revit.SharedProject.Utilities
{
    public static class SchemaUtils
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Guid SchemaId = new Guid("be3b098e-7c16-4465-92be-82624abe7f15");
        public static Schema Schema = Schema.Lookup(SchemaId);
        public static string FieldName = "Settings";

        public static StoredSettings LoadSchema(Document doc)
        {
            try
            {
                if (Schema == null)
                    Schema = CreateSchema();

                if (Schema != null)
                {
                    var storage = GetDataStorage(doc);
                    var entity = storage?.GetEntity(Schema);
                    if (entity?.Schema == null)
                        return null;

                    var json = entity.Get<string>(Schema.GetField(FieldName));
                    var settings = StoredSettings.Deserialize(json);

                    return settings;
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }

            return null;
        }

        public static Schema CreateSchema()
        {
            var builder = new SchemaBuilder(SchemaId);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Application);
            // (Konrad) Must match *.addin value set for ApplicationGUID.
            builder.SetApplicationGUID(new Guid("1a2faea3-cb50-4e75-900b-83c10420ecda"));

            // (Konrad) No spaces allowed in vendor id.
            // Must match *.addin value set for VendorId.
            builder.SetVendorId("LadybugTools");
            builder.SetDocumentation("Honeybee Revit plugin.");

            // (Konrad) No periods allowed in schema name.
            builder.SetSchemaName("Honeybee_v1_0");
            builder.AddSimpleField(FieldName, typeof(string));
            var schema = builder.Finish();

            return schema;
        }

        public static bool SaveSchema(Document doc)
        {
            var updated = false;
            using (var trans = new Transaction(doc))
            {
                trans.Start("Store Settings");
                try
                {
                    Schema = Schema ?? CreateSchema();
                    if (Schema != null)
                    {
                        var savedData = GetDataStorage(doc) ?? DataStorage.Create(doc);
                        var entity = savedData.GetEntity(Schema);
                        if (entity.Schema == null)
                            entity = new Entity(Schema);

                        var field = Schema.GetField(FieldName);
                        entity.Set<string>(field, AppSettings.Instance.StoredSettings.Serialize());
                        savedData.SetEntity(entity);

                        updated = true;
                    }

                    trans.Commit();
                }
                catch (Exception e)
                {
                    _logger.Fatal(e);
                    trans.RollBack();
                }
            }

            return updated;
        }

        #region Utilities

        private static DataStorage GetDataStorage(Document doc)
        {
            var storage = new FilteredElementCollector(doc)
                .OfClass(typeof(DataStorage))
                .Cast<DataStorage>()
                .FirstOrDefault(x =>
                {
                    var entity = x.GetEntity(Schema);
                    return entity != null && entity.IsValid();
                });

            return storage;
        }

        #endregion
    }
}
