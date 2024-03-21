using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        BarberShop shop = new BarberShop(3);
        Thread barberThread = new Thread(shop.Barber); 
        barberThread.Start(); 

        Random rand = new Random();
        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(rand.Next(100, 150));
            Thread customerThread = new Thread(shop.EnterShop); 
            customerThread.Start(); 
        }
    }
}

class BarberShop
{
    private readonly object locker = new object(); 
    private bool barberSleeping = true; 
    private int chairsAvailable; 
    private int waitingCustomers; 

    public BarberShop(int numChairs)
    {
        chairsAvailable = numChairs; 
    }

    public void EnterShop() 
    {
        lock (locker) 
        {
            if (chairsAvailable == 0) 
            {
                Console.WriteLine("Go awey from shop");
                return;
            }

            if (barberSleeping) 
            {
                Console.WriteLine("Barber has wouken up.");
                barberSleeping = false; 
                Monitor.Pulse(locker);
            }

            chairsAvailable--; 
            waitingCustomers++; 
            Console.WriteLine($"Come in to the barbershop. Weating customers: {waitingCustomers}");
        }

        Thread.Sleep(1000); 
        LeaveShop(); 
    }

    public void LeaveShop() 
    {
        lock (locker) 
        {
            waitingCustomers--; 
            chairsAvailable++; 
            Console.WriteLine($"Come out from the barbershop. Weating customers: {waitingCustomers}");
        }
    }

    public void Barber() 
    {
        while (true)
        {
            lock (locker) 
            {
                if (waitingCustomers == 0)
                {
                    barberSleeping = true; 
                    Console.WriteLine("Barber asleep.");
                    Monitor.Wait(locker); 
                }

                Console.WriteLine("Barber is working.");
            }
            Thread.Sleep(1000); 
        }
    }
}


