using System.Collections.Generic;
using System.IO;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LenovoLegionToolkit.Lib.Settings.FloatingGadgetSettings;

namespace LenovoLegionToolkit.Lib.Settings;

public class FloatingGadgetSettings() : AbstractSettings<FloatingGadgetSettingsStore>("floating_gadget.json")
{
    public class FloatingGadgetSettingsStore
    {
        public bool ShowFloatingGadgets { get; set; }
        public int FloatingGadgetsRefreshInterval { get; set; } = 1;
        public int SelectedStyleIndex { get; set; } = 0;
        public List<FloatingGadgetItem> Items { get; set; } = [];
    }

    public override FloatingGadgetSettingsStore? LoadStore()
    {
        var store = base.LoadStore();
        if (store != null)
            return store;

        try
        {
            var oldSettingsPath = Path.Combine(Folders.AppData, "settings.json");
            if (!File.Exists(oldSettingsPath))
                return null;

            var json = File.ReadAllText(oldSettingsPath);
            var jObject = JsonConvert.DeserializeObject<JObject>(json, JsonSerializerSettings);
            var itemsToken = jObject?["FloatingGadgetItems"];
            
            var migrated = new FloatingGadgetSettingsStore();

            if (jObject?["ShowFloatingGadgets"] is { } showToken)
            {
                migrated.ShowFloatingGadgets = showToken.ToObject<bool>();
            }

            if (jObject?["FloatingGadgetsRefreshInterval"] is { } intervalToken)
            {
                migrated.FloatingGadgetsRefreshInterval = intervalToken.ToObject<int>();
                if (migrated.FloatingGadgetsRefreshInterval < 1)
                    migrated.FloatingGadgetsRefreshInterval = 1;
            }

            if (jObject?["SelectedStyleIndex"] is { } styleToken)
            {
                migrated.SelectedStyleIndex = styleToken.ToObject<int>();
            }

            if (itemsToken != null)
            {
                var items = itemsToken.ToObject<List<FloatingGadgetItem>>();
                if (items is { Count: > 0 })
                    migrated.Items = items;
            }

            Store = migrated;
            Save();

            return migrated;
        }
        catch
        {
            return null;
        }
    }
}
