using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace EquipmentCatalogLibrary
{
    public class EquipmentCatalogManager
    {
        private readonly string _xmlPath;
        private readonly string _xsdPath;

        public EquipmentCatalogManager(string xmlPath, string xsdPath)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _xmlPath = System.IO.Path.Combine(baseDirectory, xmlPath);
            _xsdPath = System.IO.Path.Combine(baseDirectory, xsdPath);
        }

        public List<Equipment> LoadCatalog()
        {
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            settings.Schemas.Add(null, _xsdPath);
            settings.ValidationEventHandler += (sender, args) =>
                throw new XmlException($"Ошибка валидации: {args.Message}");

            var equipments = new List<Equipment>();
            try
            {
                using (var reader = XmlReader.Create(_xmlPath, settings))
                {
                    var doc = new XmlDocument();
                    doc.Load(reader);

                    var equipmentNodes = doc.SelectNodes("//Equipment");
                    if (equipmentNodes != null)
                    {
                        foreach (XmlNode node in equipmentNodes)
                        {
                            var equipment = new Equipment
                            {
                                Name = node["Name"]?.InnerText ?? string.Empty,
                                OwnerOrganization = node["OwnerOrganization"]?.InnerText ?? string.Empty
                            };

                            // Парсинг Cost с обработкой ошибки
                            string costText = node["Cost"]?.InnerText ?? "0";
                            if (!decimal.TryParse(costText, out decimal cost))
                            {
                                throw new FormatException($"Неверный формат стоимости для оборудования '{equipment.Name}': {costText}");
                            }
                            equipment.Cost = cost;

                            // Парсинг ManufactureYear с обработкой ошибки
                            string yearText = node["ManufactureYear"]?.InnerText ?? "0";
                            if (!int.TryParse(yearText, out int year))
                            {
                                throw new FormatException($"Неверный формат года изготовления для оборудования '{equipment.Name}': {yearText}");
                            }
                            equipment.ManufactureYear = year;

                            equipments.Add(equipment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке каталога: {ex.Message}", ex);
            }
            return equipments;
        }

        public void SaveCatalog(List<Equipment> equipments)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);

            var root = doc.CreateElement("EquipmentCatalog");
            doc.AppendChild(root);

            foreach (var eq in equipments)
            {
                var equipmentNode = doc.CreateElement("Equipment");
                equipmentNode.AppendChild(CreateElement(doc, "Name", eq.Name));
                equipmentNode.AppendChild(CreateElement(doc, "OwnerOrganization", eq.OwnerOrganization));
                equipmentNode.AppendChild(CreateElement(doc, "Cost", eq.Cost.ToString()));
                equipmentNode.AppendChild(CreateElement(doc, "ManufactureYear", eq.ManufactureYear.ToString()));
                root.AppendChild(equipmentNode);
            }

            doc.Save(_xmlPath);
        }

        private XmlElement CreateElement(XmlDocument doc, string name, string value)
        {
            var element = doc.CreateElement(name);
            element.InnerText = value;
            return element;
        }
    }
}