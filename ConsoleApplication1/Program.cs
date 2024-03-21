using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1
{
    
    class Container
    {
        private static Dictionary<string, Container> _containers = new Dictionary<string, Container>();
        private float productWeight = 0;
        private float height;
        private float weight;
        private float depth;
        private string serialNumber;
        private float maxPayload;

        public Container( float weight, float height, float depth, string type, float maxPayload)
        {
         
            this.weight = weight;
            this.height = height;
            this.depth = depth;
            this.maxPayload = maxPayload;
            SetSerialNumber(type);
            _containers.Add(serialNumber,this);
           
        }

        private void SetSerialNumber(string type)
        {
            serialNumber = "KON-" + type + "-" + _containers.Count;
        }

        
        protected void Load(float productWeight)
        {
            if (productWeight > maxPayload)
            {
                throw new OverfillException("Masa ładunku jest większa niż pojemność danego kontenera");
            }

            this.productWeight = productWeight;


        }

        protected float UnLoad()
        {
            float buffer = productWeight;
            productWeight = 0;
            return buffer;
        }
        
        protected float GetProductWeight()
        {
            return productWeight;
        }

        public string GetSerialNumber()
        {
            return serialNumber;
        }

        public override string ToString()
        {
            return "Kontener: " + serialNumber + ". Masa ładunku: " + productWeight + " kg. Сharakterystyka kontenera: Waga - " + weight + " kg, Wysokość - " + height + " cm, Głębokość - " + depth + " cm";
        }
    }
    public interface IHazardNotifier
    {
        void Alert();
    }

    class LiquidСontainer : Container, IHazardNotifier
    {
        private const string type = "L";

        private const float weight = 3000;

        private const float height = 500;

        private const float depth = 900;

        private const float maxPayload = 4000;

        private bool dangerous;
        
        private string product;
        public LiquidСontainer() : base(weight, height, depth, type, maxPayload)
        {
            
           
        }

        protected void StartLoad(float productWeight, string product, bool dangerous)
        {
            Load(productWeight);
            if (dangerous && GetProductWeight() > maxPayload*0.5f)
            {
                Alert();
            }
            else if(GetProductWeight() > maxPayload*0.9f)
            {
                Alert();
            }

            this.dangerous = dangerous;
            this.product = product;

        }

        protected float StartUnLoad()
        {
            product = null;
            return UnLoad();
            
        }
        
        public void Alert()
        {
            Console.WriteLine("Kontener: " + GetSerialNumber() + " jest w niebezpiecznej sytuacji!");
        }
    }
    
    class GasСontainer : Container, IHazardNotifier
    {
        private const string type = "G";

        private const float weight = 5000;

        private const float height = 500;

        private const float depth = 700;

        private const float maxPayload = 1000;

        private float atmosphere;
        
        private string product;
        
        public GasСontainer(float atmosphere) : base(weight, height, depth, type, maxPayload)
        {
            
            this.atmosphere = atmosphere;

        }

        protected void StartLoad(float productWeight, string product)
        {
           
            Load(productWeight);
           
           
            this.product = product;
            
        }
        
        protected float StartUnLoad()
        {
            product = null;
            return UnLoad()*0.95f;
            
        }
        public void Alert()
        {
            Console.WriteLine("Kontener: " + GetSerialNumber() + "jest w niebezpiecznej sytuacji!");
        }

        public override string ToString()
        {
            return base.ToString() + ", Ciśnienia - " + atmosphere + " at";
        }
    }
    
    class ColdСontainer : Container
    {
        private const string type = "C";

        private const float weight = 7000;

        private const float height = 400;

        private const float depth = 800;

        private const float maxPayload = 2000;

        private static Dictionary<string, float> productsTemp = new Dictionary<string, float>();

        private string product;

        private float temp;
        
        public ColdСontainer() : base( weight, height, depth, type, maxPayload)
        {
         
            InitMap();
           
            
            
        }

        protected void StartLoad(float productWeight, string product, float temp)
        {
           
            Load(productWeight);
           
            if (productsTemp[product] < temp)
            {
                throw new OverfillException("Temperatura kontenera nie jest wystarczająco niska!");
            }
            this.product = product;
            this.temp = temp;

        }
        private void InitMap()
        {
            productsTemp.Add("Bananas",13.3f);
            productsTemp.Add("Chocolate",18f);
            productsTemp.Add("Fish",2f);
            productsTemp.Add("Meat",-15f);
            productsTemp.Add("Ice cream",-18f);
            productsTemp.Add("Frozen pizza",-30f);
            productsTemp.Add("Cheese",7.2f);
            productsTemp.Add("Sausages",5f);
            productsTemp.Add("Butter",20.5f);
            productsTemp.Add("Eggs",19f);
        }

        protected float StartUnLoad()
        {
            product = null;
            return UnLoad();
            
        }
        
       
        
        public override string ToString()
        {
            return base.ToString() + ", Temperatura - " + temp + " C";
        }
    }
    
    public class OverfillException : Exception
    {
        public OverfillException(string message) : base(message) { }
    }

    class Ship
    {
        private Dictionary<string, Container> _containersShip = new Dictionary<string, Container>();

        private float speed;

        private int size;

        public Ship(float speed, int size)
        {
            this.size = size;
            this.speed = speed;
        }

        public void AddContainer(Container c)
        {
            if (_containersShip.Count != size + 1)
            {
                _containersShip.Add(c.GetSerialNumber(), c);
            }
            else
            {
                Console.WriteLine("Statek jest pelny");
            }
            
        }

        public void ListContainers()
        {
            foreach (KeyValuePair<string, Container> pair in _containersShip)
            {
                Console.WriteLine(_containersShip[pair.Key]);
            }
        }

        public static void ContainerFromTo(Ship from, Ship to, string sn)
        {
            if (from._containersShip.ContainsKey(sn))
            {
                to.AddContainer(from._containersShip[sn]);
                from._containersShip.Remove(sn);
               
            }
        }

        public void Exchange(Ship s, Container c, string sn)
        {
            if (_containersShip.ContainsKey(sn))
            {
                s.AddContainer(_containersShip[sn]);
                _containersShip.Remove(sn);
                this.AddContainer(c);
            }

        }

        public Dictionary<string, Container> UnLoadShip()
        {
            Dictionary<string, Container> buffer = _containersShip;
            _containersShip = new Dictionary<string, Container>();
            return buffer;
        }

        public Container UnLoadContainer(string sn)
        {
            Container buffer = _containersShip[sn];
            _containersShip.Remove(sn);
            return buffer;
        }

        public override string ToString()
        {
            ListContainers();
            return "|STATEK INFO| Prędkość statku: " + speed + ". Maksymalna liczba kontenerów: " + size + ". Liczba kontenerów:" + _containersShip.Count;
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            
        }
    }
}