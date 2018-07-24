using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace AggregateGDPPopulation
{
    public class Class1
    {
        Dictionary<string, string> Convert;
        Dictionary<string, Dictionary<string, float>> JSONContainer;

        public Class1()
        {
            JSONContainer = new Dictionary<string, Dictionary<string, float>>();
        }

        public static async Task<string> FileRead(string FilePath)
        {
            using (StreamReader sr = new StreamReader(FilePath))
            {
               string ReadFile = await sr.ReadToEndAsync();
               return ReadFile;
            }
        }

        public static async Task FileWrite(string FilePath, string data)
        {
            using (StreamWriter sr = new StreamWriter(FilePath))
            {
                await sr.WriteAsync(data);
            }
        }

        public Dictionary<string, string> ParseMapper(string countryContinentJson)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            try
            {
                mapper = JsonConvert.DeserializeObject<Dictionary<string, string>>(countryContinentJson);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in parsing the mapper file: {}", e.Message);
            }
            return mapper;
        }

        public async void AggregateData()
        {
            string Data = await FileRead(@"..\..\..\..\AggregateGDPPopulation\data\datafile.csv");
            string MapperData = await FileRead(@"..\..\..\..\AggregateGDPPopulation\data\country-list.json");
            Convert = ParseMapper(MapperData);
            string[] DataString = Data.Replace("\"", String.Empty).Trim().Split('\n');
            string[] HeaderValue = DataString[0].Split(',');
            int CountryIndex = Array.IndexOf(HeaderValue, "Country Name");
            int GDPIndex = Array.IndexOf(HeaderValue, "GDP Billions (US Dollar) - 2012");
            int PopulationIndex = Array.IndexOf(HeaderValue, "Population (Millions) - 2012");


            string[] trial = new string[DataString.Length];
            for (int i = 1; i < DataString.Length; i++)
            {
                trial = DataString[i].Split(',');
                string CountryName = trial[CountryIndex];
                float GDPvalue = float.Parse(trial[GDPIndex]);
                float PopulationValue = float.Parse(trial[PopulationIndex]);
                if (Convert.ContainsKey(CountryName))
                {
                    string ContinentName = Convert[CountryName];

                    if (JSONContainer.ContainsKey(ContinentName))
                    {
                        Dictionary<string, float> continentValues = JSONContainer[ContinentName];
                        float GDP_2012 = continentValues["GDP_2012"];
                        continentValues.Remove("GDP_2012");
                        continentValues.Add("GDP_2012", GDP_2012 + GDPvalue);
                        float POPULATION_2012 = continentValues["POPULATION_2012"];
                        continentValues.Remove("POPULATION_2012");
                        continentValues.Add("POPULATION_2012", POPULATION_2012 + PopulationValue);
                        JSONContainer.Remove(ContinentName);
                        JSONContainer.Add(ContinentName, continentValues);
                    }
                    else
                    {
                        Dictionary<string, float> continentValues = new Dictionary<string, float>();
                        continentValues.Add("GDP_2012", GDPvalue);
                        continentValues.Add("POPULATION_2012", PopulationValue);
                        JSONContainer.Add(ContinentName, continentValues);
                    }
                }
            }

            await FileWrite("output/output.json", JsonConvert.SerializeObject(JSONContainer));
        }
    }




    
}