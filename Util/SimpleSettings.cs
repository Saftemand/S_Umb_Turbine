using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.IO;
using System.Configuration;

namespace RumblingRhino.Util
{    
    [Serializable]
    public class ApplicationSettings
    {
        public static string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationSettings.AppSettings["RumblingRhinoSettingsXmlPath"];
        
        public string DBConnectionString { get; set; }

        public bool Debug { get; set; }
                
        private static readonly XmlSerializer serial = new XmlSerializer(typeof(ApplicationSettings));
        public static ApplicationSettings Instance {
            get
            {  
                string cacheKey = "SettingsCache";                
                if (HttpContext.Current != null)
                {   
                    if (HttpContext.Current.Cache[cacheKey] != null) return (ApplicationSettings)HttpContext.Current.Cache[cacheKey];                                        
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(settingsFilePath);
                using (XmlReader reader = new XmlNodeReader(doc))
                {
                    ApplicationSettings appSettings = Deserialize<ApplicationSettings>(reader);
                    if (HttpContext.Current != null)
                    {
                        CacheDependency dep = new CacheDependency(settingsFilePath);
                        HttpContext.Current.Cache.Insert(cacheKey, appSettings, dep, DateTime.MaxValue, Cache.NoSlidingExpiration);
                    }
                    return appSettings;

                }
            }
        }        
        private static T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(reader);
        }


    }
}
