using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using LitJson;

public class TravelData
{
    public string Theme;
    public string Name;
    public string CountryName;

    public string Name_kr;
    public string CountryName_kr;
    public int idx;
}

public class QuizData
{
    public string Stage;
    public string Imagepath;
    public string ImageName;
}
public class JsonHelper : MonoBehaviour {
    public static JsonHelper Instance;
    public QuizData[] _data;
    public int QuizLevel;
    void Awake()
    {
        Instance = this;
        QuizDataSave();
        TravelDataSave();
    }
    static void QuizDataSave()
    {
        JsonData jsondata =new JsonData();
        //"이미지 경로/국가명"
        jsondata["FlagSt1"] = new JsonData();
        jsondata["FlagSt1"]["0"] = "GE#UnitedStates/미국/";
        jsondata["FlagSt1"]["1"] = "GE#Korea/대한민국/";
        jsondata["FlagSt1"]["2"] = "GE#China/중국/";
        jsondata["FlagSt1"]["3"] = "GE#Canada/캐나다/";
        jsondata["FlagSt1"]["4"] = "GE#England/영국/";

        jsondata["FlagSt2"] = new JsonData();
        jsondata["FlagSt2"]["0"] = "GE#England/영국/";
        jsondata["FlagSt2"]["1"] = "GE#France/프랑스/";
        jsondata["FlagSt2"]["2"] = "GE#Germany/독일/";
        jsondata["FlagSt2"]["3"] = "GE#NorthKorea/북한/";
        jsondata["FlagSt2"]["4"] = "GE#Japan/일본/";
        jsondata["FlagSt2"]["5"] = "GE#Greece/그리스/";
        jsondata["FlagSt2"]["6"] = "GE#Canada/캐나다/";
        jsondata["FlagSt2"]["7"] = "GE#Thailand/태국/";
        jsondata["FlagSt2"]["8"] = "GE#Singapore/싱가포르/";
        jsondata["FlagSt2"]["9"] = "GE#Brazil/브라질/";

        jsondata["FlagSt3"] = new JsonData();
        jsondata["FlagSt3"]["0"] = "GE#UnitedStates/미국/";
        jsondata["FlagSt3"]["1"] = "GE#Greece/그리스/";
        jsondata["FlagSt3"]["2"] = "GE#France/프랑스/";
        jsondata["FlagSt3"]["3"] = "GE#Germany/독일/";
        jsondata["FlagSt3"]["4"] = "GE#India/인도/";
        jsondata["FlagSt3"]["5"] = "GE#Japan/일본/";
        jsondata["FlagSt3"]["6"] = "GE#Russia/러시아/";
        jsondata["FlagSt3"]["7"] = "GE#Netherlands/네덜란드/";
        jsondata["FlagSt3"]["8"] = "GE#Swiss/스위스/";
        jsondata["FlagSt3"]["9"] = "GE#Nepal/네팔/";
        jsondata["FlagSt3"]["10"] = "GE#Spain/스페인/";
        jsondata["FlagSt3"]["11"] = "GE#Brazil/브라질/";
        jsondata["FlagSt3"]["12"] = "GE#Mexico/멕시코/";

        jsondata["ClothSt1"] = new JsonData();
        jsondata["ClothSt1"]["0"] = "GN#China/중국/";
        jsondata["ClothSt1"]["1"] = "GN#Korea/대한민국/";
        jsondata["ClothSt1"]["2"] = "GN#UnitedStates/미국/";
        jsondata["ClothSt1"]["3"] = "GN#Japan/일본/";
        jsondata["ClothSt1"]["4"] = "GN#Mongolia/몽골/";

        jsondata["ClothSt2"] = new JsonData();
        jsondata["ClothSt2"]["0"] = "GN#England/영국/";
        jsondata["ClothSt2"]["1"] = "GN#Egypt/이집트/";
        jsondata["ClothSt2"]["2"] = "GN#UnitedStates/미국/";
        jsondata["ClothSt2"]["3"] = "GN#Iran/이란/";
        jsondata["ClothSt2"]["4"] = "GN#France/프랑스/";
        jsondata["ClothSt2"]["5"] = "GN#China/중국/";
        jsondata["ClothSt2"]["6"] = "GN#Korea/대한민국/";
        jsondata["ClothSt2"]["7"] = "GN#Vietnam/베트남/";
        jsondata["ClothSt2"]["8"] = "GN#Japan/일본/";
        jsondata["ClothSt2"]["9"] = "GN#Mongolia/몽골/";

        jsondata["ClothSt3"] = new JsonData();
        jsondata["ClothSt3"]["0"] = "GN#England/영국/";
        jsondata["ClothSt3"]["1"] = "GN#Egypt/이집트/";
        jsondata["ClothSt3"]["2"] = "GN#UnitedStates/미국/";
        jsondata["ClothSt3"]["3"] = "GN#Iran/이란/";
        jsondata["ClothSt3"]["4"] = "GN#France/프랑스/";
        jsondata["ClothSt3"]["5"] = "GN#China/중국/";
        jsondata["ClothSt3"]["6"] = "GN#Korea/대한민국/";
        jsondata["ClothSt3"]["7"] = "GN#Vietnam/베트남/";
        jsondata["ClothSt3"]["8"] = "GN#Japan/일본/";
        jsondata["ClothSt3"]["9"] = "GN#Mongolia/몽골/";
        jsondata["ClothSt3"]["10"] = "GN#Mexico/멕시코/";
        jsondata["ClothSt3"]["11"] = "GN#Russia/러시아/";
        jsondata["ClothSt3"]["12"] = "GN#Greece/그리스/";
        jsondata["ClothSt3"]["13"] = "GN#Thailand/태국/";


        jsondata["LandMarkSt1"] = new JsonData();
        jsondata["LandMarkSt1"]["0"] = "GH#WhiteHouse/백악관/";
        jsondata["LandMarkSt1"]["1"] = "GH#BlueHouse/청와대/";
        jsondata["LandMarkSt1"]["2"] = "GH#LondonEye/런던아이/";
        jsondata["LandMarkSt1"]["3"] = "GH#EiffelTower/에펠탑/";
        jsondata["LandMarkSt1"]["4"] = "GH#Colosseum/콜로세움/";
        jsondata["LandMarkSt1"]["5"] = "GH#TowerBridge/타워브릿지/";

        jsondata["LandMarkSt2"] = new JsonData();
        jsondata["LandMarkSt2"]["0"] = "GH#EiffelTower/에펠탑/"; 
        jsondata["LandMarkSt2"]["1"] = "GH#AngkorWat/앙코르와트/";
        jsondata["LandMarkSt2"]["2"] = "GH#NamsanTower/남산타워/";
        jsondata["LandMarkSt2"]["3"] = "GH#Brandenburg/브란덴브루크/";
        jsondata["LandMarkSt2"]["4"] = "GH#Pyramid/피라미드/";
        jsondata["LandMarkSt2"]["5"] = "GH#Windmill/풍차/";
        jsondata["LandMarkSt2"]["6"] = "GH#BurjKhalifa/부르즈할리파/";
        jsondata["LandMarkSt2"]["7"] = "GH#StatueOfLiberty/자유의여신상/";
        jsondata["LandMarkSt2"]["8"] = "GH#TheGreatWall/만리장성/";
        jsondata["LandMarkSt2"]["9"] = "GH#OperaHouse/오페라하우스/";
        jsondata["LandMarkSt2"]["10"] = "GH#BloodTemple/피의사원/";

        jsondata["LandMarkSt3"] = new JsonData();
        jsondata["LandMarkSt3"]["0"] = "GH#EiffelTower/에펠탑/";
        jsondata["LandMarkSt3"]["1"] = "GH#AngkorWat/앙코르와트/";
        jsondata["LandMarkSt3"]["2"] = "GH#NamsanTower/남산타워/";
        jsondata["LandMarkSt3"]["3"] = "GH#Brandenburg/브란덴브루크/";
        jsondata["LandMarkSt3"]["4"] = "GH#Pyramid/피라미드/";
        jsondata["LandMarkSt3"]["5"] = "GH#Windmill/풍차/";
        jsondata["LandMarkSt3"]["6"] = "GH#BurjKhalifa/부르즈할리파/";
        jsondata["LandMarkSt3"]["7"] = "GH#StatueOfLiberty/자유의여신상/";
        jsondata["LandMarkSt3"]["8"] = "GH#TheGreatWall/만리장성/";
        jsondata["LandMarkSt3"]["9"] = "GH#OperaHouse/오페라하우스/";
        jsondata["LandMarkSt3"]["10"] = "GH#BloodTemple/피의사원/";
        jsondata["LandMarkSt3"]["11"] = "GH#TajMahal/타지마할/";
        jsondata["LandMarkSt3"]["12"] = "GH#63Building/63빌딩/";
        jsondata["LandMarkSt3"]["13"] = "GH#AthensCastle/아테네성/";
        jsondata["LandMarkSt3"]["14"] = "GH#BulguksaTemple/불국사/";
        jsondata["LandMarkSt3"]["15"] = "GH#TokyoTower/도쿄타워/";
        string jsonText = jsondata.ToJson();

       // Debug.Log(jsonText);
        File.WriteAllText(Application.dataPath + "/jsonText2.json", jsonText.ToString());
        
    }
    string GetJsonKey(ML.SportsMiniGame.KinectSkating.Stage stage)
    {
        string key = "";
        if (QuizLevel == 1)
        {
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage1)
            {
                key = "FlagSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage2)
            {
                key = "FlagSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage3)
            {
                key = "FlagSt3";
            }
        }
        else if (QuizLevel == 2)
        {
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage1)
            {
                key = "ClothSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage2)
            {
                key = "ClothSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage3)
            {
                key = "ClothSt3";
            }
        }
        else if (QuizLevel == 3)
        {
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage1)
            {
                key = "LandMarkSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage2)
            {
                key = "LandMarkSt3";
            }
            if (stage == ML.SportsMiniGame.KinectSkating.Stage.Stage3)
            {
                key = "LandMarkSt3";
            }
        }
        return key;
    }
   
    public QuizData[] Load(ML.SportsMiniGame.KinectSkating.Stage stage)
    {
        string Key = GetJsonKey(stage);
        //Debug.Log("불러올 퀴즈의 Key값은" + Key);
        try
        {
            string jsonString = File.ReadAllText(Application.dataPath + "/jsonText2.json");
            JsonData getjson = JsonMapper.ToObject(jsonString);
            //Debug.Log("퀴즈 로딩완료: " +getjson[Key].Count + "개의 퀴즈 로우데이터가 발견되었습니다.");
            QuizData[] data = new QuizData[getjson[Key].Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new QuizData();
                data[i].Stage = Key;
                string tmp = getjson[Key][i.ToString()].ToString();
                string[] sp = tmp.Split('/');
                data[i].Imagepath = sp[0];
                data[i].ImageName = sp[1];
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.Log("퀴즈 로딩 에러");
            Debug.Log("Error:" + e.Message);
        }

        return null;
    }

    static void TravelDataSave()
    {
        JsonData jsondata = new JsonData();
        jsondata["Culture"] = new JsonData();
        jsondata["Culture"]["0"] = "Forum Romanum/Italy/포로 로마노/이탈리아/5";
        jsondata["Culture"]["1"] = "Forum Romanum/Italy/포로 로마노/이탈리아/5";
        jsondata["Culture"]["2"] = "Pisa/Italy/피사의 사탑/이탈리아/5";
        jsondata["Culture"]["3"] = "Pisa/Italy/피사의 사탑/이탈리아/5";
        jsondata["Culture"]["4"] = "BigBen/England/빅 벤/영국/7";
        jsondata["Culture"]["5"] = "StatueOfLiberty/USA/자유의 여신상/미국/10";
        jsondata["Culture"]["6"] = "Colosseum/Italy/콜로세움/이탈리아/5";
        jsondata["Culture"]["7"] = "Cambridge/England/캠브리지 대학교/영국/7";
        jsondata["Culture"]["8"] = "Eiffel Tower/France/에펠탑/프랑스/6";

        jsondata["Art"] = new JsonData();
        jsondata["Art"]["0"] = "Arc De Triomphe/France/개선문/프랑스/6";
        jsondata["Art"]["1"] = "NotreDame/France/노트르담 성당/프랑스/6";
        jsondata["Art"]["2"] = "StatueOfLiberty/USA/자유의 여신상/미국/10";
        jsondata["Art"]["3"] = "St. Pauls Cathedral/England/세인트 폴 대성당/영국/7";
        jsondata["Art"]["4"] = "Parthenon Shrine/Greece/파르테논 신전/그리스/5";
        jsondata["Art"]["5"] = "Spinks/Egypt/스핑크스/이집트/3";
        jsondata["Art"]["6"] = "Trevi Fountain/Italy/트레빌 광장/이탈리아/5";
        jsondata["Art"]["7"] = "St. Peters Cathedral/Italy/성 베드로 대성당/이탈리아/5";
        jsondata["Art"]["8"] = "Ofera/France/오페라 가르디에/프랑스/6";

        jsondata["City"] = new JsonData();
        jsondata["City"]["0"] = "Newyork/USA/뉴욕/미국/10";
        jsondata["City"]["1"] = "Rome/Italy/로마/이탈리아/5";
        jsondata["City"]["2"] = "Paris/France/파리/프랑스/6";
        jsondata["City"]["3"] = "London/England/런던/영국/7";
        jsondata["City"]["4"] = "Athens/Greece/아테네/그리스/5";
        jsondata["City"]["5"] = "Tokyo/Japan/도쿄/일본/0";
        jsondata["City"]["6"] = "Santorini/Greece/산토리니/그리스/5";
        jsondata["City"]["7"] = "Cambridge/England/캠브리지 대학교/영국/7";
        jsondata["City"]["8"] = "Empty/Empty/Empty/Empty/0";

        jsondata["Tour"] = new JsonData();
        jsondata["Tour"]["0"] = "Tower Bridge/England/타워 브리지/영국/7";
        jsondata["Tour"]["1"] = "Disneyland/Japan/디즈니랜드/일본/0";
        jsondata["Tour"]["2"] = "Brooklyn Dumbo/USA/브루클린 덤보/미국/10";
        jsondata["Tour"]["3"] = "Brooklyn Dumbo/USA/브루클린 덤보/미국/10";
        jsondata["Tour"]["4"] = "Burj Khalifa/UAE/부르즈할리파/아랍에미리트/3";
        jsondata["Tour"]["5"] = "London Eye/England/런던아이/영국/7";
        jsondata["Tour"]["6"] = "Hollywood/USA/할리우드 유니버셜/미국/8";
        jsondata["Tour"]["7"] = "Hollywood/USA/할리우드 유니버셜/미국/8";
        jsondata["Tour"]["8"] = "Empty/Empty/Empty/Empty/0";

        jsondata["Nature"] = new JsonData();
        jsondata["Nature"]["0"] = "Grand Canyon/USA/그랜드 캐니언/미국/9";
        jsondata["Nature"]["1"] = "Seongsan Sunrise Peak/Korea/성산일출봉/한국/0";
        jsondata["Nature"]["2"] = "Sky Pasture/Korea/대관령 스카이목장/한국/0";
        jsondata["Nature"]["3"] = "Suncheon bay/Korea/순천만/한국/0";
        jsondata["Nature"]["4"] = "Arab Desert/UAE/아랍사막/아랍에미리트/3";
        jsondata["Nature"]["5"] = "Goseong/Korea/고성 공룡박물관/한국/0";
        jsondata["Nature"]["6"] = "Santorini/Greece/산토리니/그리스/5";
        jsondata["Nature"]["7"] = "Empty/Empty/Empty/Empty/0";
        jsondata["Nature"]["8"] = "Empty/Empty/Empty/Empty/0";

        jsondata["History"] = new JsonData();
        jsondata["History"]["0"] = "Angkor Wat/Cambodia/앙코르와트/캄보디아/1";
        jsondata["History"]["1"] = "TajMahal/India/타지마할/인도/2";
        jsondata["History"]["2"] = "The Great Wall/China/만리장성/중국/0";
        jsondata["History"]["3"] = "Agra Fortress/India/아그라 요새/인도/2";
        jsondata["History"]["4"] = "Agra Fortress/India/아그라 요새/인도/2";
        jsondata["History"]["5"] = "Venice/Italy/베네치아/이탈리아/5";
        jsondata["History"]["6"] = "Venice/Italy/베네치아/이탈리아/5";
        jsondata["History"]["7"] = "Trafalgar Square/England/트라팔가 광장/영국/7";
        jsondata["History"]["8"] = "Trafalgar Square/England/트라팔가 광장/영국/7";

        string jsonText = jsondata.ToJson();

        // Debug.Log(jsonText);
        File.WriteAllText(Application.dataPath + "/jsonText.json", jsonText.ToString());

    }
    public TravelData[] Load(string Key)
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/jsonText.json");
        JsonData getjson = JsonMapper.ToObject(jsonString);
        TravelData[] data = new TravelData[9];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new TravelData();
            data[i].Theme = Key;
            string tmp = getjson[Key][i.ToString()].ToString();
            string[] sp = tmp.Split('/');
            data[i].Name = sp[0];
            data[i].CountryName = sp[1];
            data[i].Name_kr = sp[2];
            data[i].CountryName_kr = sp[3];
            data[i].idx = int.Parse(sp[4]);
        }
        return data;
    }
}