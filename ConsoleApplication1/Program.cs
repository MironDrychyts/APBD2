using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Product
    {
        private string name;
        private float productWeight;
        private bool dangerous;

        public Product(string name, float productWeight, bool dangerous)
        {
            this.name = name;
            this.productWeight = productWeight;
            this.dangerous = dangerous;
        }

        public string GetName()
        {
            return name;
        }

        public float GetProductWeight()
        {
            return productWeight;
        }

        public void SetProductWeight(float weight)
        {
            productWeight = weight;
        }
        
        public bool IsDangerous()
        {
            return dangerous;
        }
    }
    class Container
    {
        private static Dictionary<string, Container> _containers = new Dictionary<string, Container>();
        private float height;
        private float weight;
        private float depth;
        private string serialNumber;
        private float maxPayload;
        private Product product;

        public Container( float weight, float height, float depth, string type, float maxPayload)
        {
            product = null;
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

        
        public virtual void Load(Product product)
        {
            if (this.product != null)
            {
                Console.WriteLine("Kontener już zawiera: " + this.product.GetName());
                return;
            }
            if (product.GetProductWeight() > maxPayload)
            {
                throw new OverfillException("Masa ładunku jest większa niż pojemność danego kontenera");
            }

            this.product = product;
            


        }

        public virtual Product UnLoad()
        {
            Product buffer = product;
            product = null;
            return buffer;
        }
        
        
        protected float GetProductWeight()
        {
            if (product != null)
            {
                return product.GetProductWeight();
            }
            
            return 0;
            
        }

        public static Dictionary<string, Container> GetContainers()
        {
            return _containers;
        }
        protected string GetProductName()
        {
            if (product != null)
            {
                return product.GetName();
            }
            
            return "Empty";
            
        }

        public string GetSerialNumber()
        {
            return serialNumber;
        }

       

        public override string ToString()
        {
            return "Kontener: " + serialNumber + ". Ładunek: " + GetProductName() + ". Masa ładunku: " + GetProductWeight() + " kg. Сharakterystyka kontenera: Waga - " + weight + " kg, Wysokość - " + height + " cm, Głębokość - " + depth + " cm";
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

        private const float maxPayload = 2000;
        
       
        public LiquidСontainer() : base(weight, height, depth, type, maxPayload)
        {
            
           
        }

        public override void Load(Product product)
        {
            base.Load(product);
            if (product.IsDangerous() && product.GetProductWeight() > maxPayload*0.5f)
            {
                Alert();
            }
            else if(product.GetProductWeight() > maxPayload*0.9f)
            {
                Alert();
            }

            

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
        
    
        
        public GasСontainer(float atmosphere) : base(weight, height, depth, type, maxPayload)
        {
            
            this.atmosphere = atmosphere;

        }
        


        public override Product UnLoad()
        {
            Product buffer = base.UnLoad();
            float newWeight = (buffer.GetProductWeight()*0.95f);
            buffer.SetProductWeight(newWeight);
            return buffer;
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

        private ListColdContainer productsTemp;
        
        private float temp;
        
        public ColdСontainer(float temp) : base( weight, height, depth, type, maxPayload)
        {
            this.temp = temp;
            productsTemp = new ListColdContainer();


        }

       

        public override void Load(Product product)
        {
            if (!productsTemp.GetList().ContainsKey(product.GetName()))
            {
                Console.WriteLine("Ten produkt nie może być przechowywany w tym kontenerze");
                return;
            }
            
            
            if (productsTemp.GetList()[product.GetName()] < temp)
            {
                throw new OverfillException("Temperatura kontenera nie jest wystarczająco niska!");
            }
            base.Load(product);
            
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

            if (!_containersShip.ContainsKey(c.GetSerialNumber()))
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

        }

        public void AddList(List<Container> containers)
        {
            foreach (Container container in containers)
            {
                AddContainer(container);
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

        public void Exchange(Ship s, string snFrom, string snTo)
        {
            if (s._containersShip.ContainsKey(snFrom))
            {
                if (_containersShip.ContainsKey(snTo))
                {
                    s.AddContainer(_containersShip[snTo]);
                    _containersShip.Remove(snTo);
                    AddContainer(s._containersShip[snFrom]);
                    s._containersShip.Remove(snFrom);
                }
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
            return "|STATEK INFO| Prędkość statku: " + speed + " węzłów. Maksymalna liczba kontenerów: " + size + ". Liczba kontenerów: " + _containersShip.Count;
        }
    }

    class ListColdContainer
    {
        private static Dictionary<string, float> productsTemp = new Dictionary<string, float>();

        public ListColdContainer()
        {
            InitMap();
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

        public Dictionary<string, float> GetList()
        {
            return productsTemp;
        }
        
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
       
            Container coldCont1 = new ColdСontainer(10);
            Product p1 = new Product("Bananas", 1500, false);
            coldCont1.Load(p1);
            Console.WriteLine(coldCont1);
            Container liquidCont1 = new LiquidСontainer();
            liquidCont1.Load(new Product("paliwo",1110,true));
            liquidCont1.Load(new Product("mleko",500,false));
            Container gasCont1 = new GasСontainer(5);
            Product p2 = new Product("gas", 400, true);
            gasCont1.Load(p2);
            p2 = gasCont1.UnLoad();
            Console.WriteLine(p2.GetProductWeight());

            Ship ship1 = new Ship(5, 15);

            List<Container> containers = new List<Container>();
            containers.Add(coldCont1);
            containers.Add(liquidCont1);
            ship1.AddList(containers);
            
            Console.WriteLine(ship1);
            
            gasCont1.Load(p2);
            Console.WriteLine(gasCont1);

            Ship ship2 = new Ship(7, 17);
            
            ship2.AddContainer(gasCont1);
            
            Console.WriteLine("S1____________________________");
            ship1.ListContainers();
            Console.WriteLine("S2____________________________");
            ship2.ListContainers();
             
            ship1.Exchange(ship2,"KON-G-2","KON-L-1");
            
            Console.WriteLine("S1____________________________");
            ship1.ListContainers();
            Console.WriteLine("S2____________________________");
            ship2.ListContainers();

            Dictionary<string, Container> containersShip = ship1.UnLoadShip();
            Console.WriteLine("____________________________");
            foreach (KeyValuePair<string, Container> pair in containersShip)
            {
                Console.WriteLine(containersShip[pair.Key]);
            }
            Console.WriteLine("____________________________");
            ship1.ListContainers();
            
            






        }
    }
}