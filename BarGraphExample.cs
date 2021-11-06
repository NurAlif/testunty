using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;
using BarGraph.VittorCloud;
using Newtonsoft.Json;

using WebSocketSharp;

public class BarGraphExample : MonoBehaviour
{
    [HideInInspector]
    public List<BarGraphDataSet> exampleDataSet;
    BarGraphGenerator barGraphGenerator;

    private bool initiated = false;
    private List<BarGraphDataSet> listBarGraph;
    private List<BarGraphDataSet> updatingListBar;


    [SerializeField]
    private Material material;

    [SerializeField]
    private Color color;

    WebSocket ws;

    public void UpdateDataChart(List<BarGraphDataSet> data)
    {

        if (data.Count == 0)
        {
            Debug.Log("asdasdasdaasdasd");
            return;
        }
        updatingListBar = data;
    }

    public void InitDataChart(List<BarGraphDataSet> data)
    {
        exampleDataSet = data;
        updatingListBar = data;

        if (exampleDataSet.Count == 0)
        {
            Debug.LogError("ExampleDataSet is Empty!");
        }

        //barGraphGenerator.GeneratBarGraph(exampleDataSet);

    }

    void Start()
    {

        barGraphGenerator = GetComponent<BarGraphGenerator>();

        ws = new WebSocket("ws://localhost:8081");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            string dataStr = e.Data;

            Debug.Log(dataStr);

            List<SimpleDataModel> data = JsonConvert.DeserializeObject<List<SimpleDataModel>>(dataStr);

            BarGraphDataSet dataBarGraph = new BarGraphDataSet();
            List<XYBarValues> barValues = new List<XYBarValues>();

            data.ForEach(item => {
                XYBarValues barValue = new XYBarValues();
                barValue.XValue = item.name;
                barValue.YValue = item.count;
                barValues.Add(barValue);
            });

            dataBarGraph.ListOfBars = barValues;

            if (!initiated)
            {
                listBarGraph = new List<BarGraphDataSet>();

                dataBarGraph.GroupName = "SimpleData";
                dataBarGraph.barColor = color;
                dataBarGraph.barMaterial = material;

                initiated = true;
                listBarGraph.Add(dataBarGraph);
                this.InitDataChart(listBarGraph);
            }
            else
            {
                Debug.Log("Update");
                List<BarGraphDataSet> listBarGraph2 = new List<BarGraphDataSet>();

                listBarGraph2.Add(dataBarGraph);
                this.UpdateDataChart(listBarGraph2);
            }
        };

        StartCoroutine(CreateDataSet());
    }


    /*
    //call when the graph starting animation completed,  for updating the data on run time
    public void StartUpdatingGraph()
    {

       
        StartCoroutine(CreateDataSet());
    }

    */
    
    IEnumerator CreateDataSet()
    {
        yield return new WaitForSeconds(3.0f);

        barGraphGenerator.GeneratBarGraph(exampleDataSet);

        while(true)
        {
            List<XYBarValues> barValues = exampleDataSet[0].ListOfBars;
            List<XYBarValues> barValuesInput = updatingListBar[0].ListOfBars;

            for (int i = 0; i < barValues.Count; i++)
            {
                if (barValuesInput[i] == barValues[i]) continue;
                Debug.Log("22");
                barValues[i] = barValuesInput[i];
                yield return new WaitForSeconds(.5f);
                barGraphGenerator.AddNewDataSet(0, i, barValues[i].YValue);

            }

            yield return new WaitForSeconds(.5f);
        }
    }
    
    



    //Generates the random data for the created bars
    void GenerateRandomData()
    {
        int dataSetIndex = UnityEngine.Random.Range(0, exampleDataSet.Count);
        int xyValueIndex = UnityEngine.Random.Range(0, exampleDataSet[dataSetIndex].ListOfBars.Count);
        exampleDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue = UnityEngine.Random.Range(barGraphGenerator.yMinValue, barGraphGenerator.yMaxValue);
        barGraphGenerator.AddNewDataSet(dataSetIndex, xyValueIndex, exampleDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue);
    }
    
}



