using System;
using System.Threading;

namespace Jogo3D_Terminal
{
    class Program
    {
        static int screenX = 120;
        static int screenY = 29;
        static char[] screen = new char[screenX * screenY];
        public static Player player = new Player();
        public static Mapa mapa = new Mapa();
        public static int escala = 20;

        public class Player
        {
            public float x = 500;
            public float y = 36;
            public float angRad = -4;
            public float angVisao = (float) Math.PI / 3;

            public void move(int qtd)
            {
                float newy = y + (float)(Math.Sin(angRad) * qtd);
                float newx = x + (float)(Math.Cos(angRad) * qtd);
                if (!((newy / escala) >= mapa.altura || (newy / escala) <= 0))
                {
                    y += (float)(Math.Sin(angRad) * qtd);
                }
                if (!((newx / escala) >= mapa.largura || (newx / escala) <= 0))
                {
                    x += (float)(Math.Cos(angRad) * qtd);
                }
            }
        }

        public class Mapa
        {
            public int altura = 0;
            public int largura = 20;
            public char[] estrutura;

            public Mapa()
            {
                string estruturaString;
                string temp;
                estruturaString  = temp = "###########################"; altura++;
                estruturaString += temp = "#.........................#"; altura++;
                estruturaString += temp = "#..########...............#"; altura++;
                estruturaString += temp = "#..#.....######...........#"; altura++;
                estruturaString += temp = "#..#......................#"; altura++;
                estruturaString += temp = "#..#......................#"; altura++;
                estruturaString += temp = "#..#........#.............#"; altura++;
                estruturaString += temp = "#..#......................#"; altura++;
                estruturaString += temp = "#...................#.....#"; altura++;
                estruturaString += temp = "####........#.......#.....#"; altura++;
                estruturaString += temp = "####......................#"; altura++;
                estruturaString += temp = "###########################"; altura++;

                largura = temp.Length;

                estrutura = estruturaString.ToCharArray();
            }
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();
            update();
            render();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.W)
                    {
                        player.move(6);
                        update();
                    }
                    if (key.Key == ConsoleKey.S)
                    {
                        player.move(-6);
                        update();
                    }
                    if (key.Key == ConsoleKey.D)
                    {
                        player.angRad += 0.04f;
                        update();
                    }
                    if (key.Key == ConsoleKey.A)
                    {
                        player.angRad -= 0.04f;
                        update();
                    }
                }

                Thread.Sleep(1);
            }
        }



        public static void update()
        {
            UpdateMapa();
            render();
        }

        private static void UpdateMapa()
        {
            for (int x = 0; x < mapa.largura; x++)
            {
                for (int y = 0; y < mapa.altura; y++)
                {
                    screen[screenX * y + x] = mapa.estrutura[mapa.largura * y + x];
                }
            }
            screen[screenX * ((int)player.y / escala) + ((int)player.x / escala)] = 'O';
        }

        public static void calculaMiniMapa()
        {
            bool bateuParede = false;
            int distanciaParede = 0;

            float caminhoX = (float)Math.Cos(player.angRad);
            float caminhoY = (float)Math.Sin(player.angRad);
            float lugarAtualX = player.x;
            float lugarAtualY = player.y;
            int passo = escala;
            int deep = escala * 20;

            while (!bateuParede)
            {

                if (screen[screenX * (int)(lugarAtualY / escala) + (int)(lugarAtualX / escala)] == '#')
                {
                    bateuParede = true;
                }
                else
                {
                    lugarAtualX += caminhoX * passo;
                    lugarAtualY += caminhoY * passo;
                    distanciaParede += passo;

                    if (lugarAtualX < 0) lugarAtualX = 0;
                    if (lugarAtualX > mapa.largura * escala) lugarAtualX = mapa.largura * escala;
                    if (lugarAtualY < 0) lugarAtualY = 0;
                    if (lugarAtualY > mapa.largura * escala) lugarAtualY = mapa.largura * escala;

                    char charAtual = screen[screenX * (int)(lugarAtualY / escala) + (int)(lugarAtualX / escala)];
                    if (charAtual == '.')
                    {
                        screen[screenX * (int)(lugarAtualY / escala) + (int)(lugarAtualX / escala)] = '*';
                    }
                }
            }
        }

        public static int distanciaParede(float anguloRadiano)
        {
            bool bateuParede = false;
            int distanciaParede = 0;

            float caminhoX = (float)Math.Cos(anguloRadiano);
            float caminhoY = (float)Math.Sin(anguloRadiano);
            float lugarAtualX = player.x;
            float lugarAtualY = player.y;
            int passo = 1;
            int deep = escala * 20;

            while (!bateuParede)
            {

                if (screen[screenX * (int)(lugarAtualY / escala) + (int)(lugarAtualX / escala)] == '#')
                {
                    bateuParede = true;
                }
                else
                {
                    lugarAtualX += caminhoX * passo;
                    lugarAtualY += caminhoY * passo;
                    distanciaParede += passo;

                    if (lugarAtualX < 0)
                    {
                        lugarAtualX = 0;
                        bateuParede = true;
                    }
                    if (lugarAtualX > mapa.largura * escala)
                    {
                        lugarAtualX = mapa.largura * escala;
                        bateuParede = true;
                    }
                    if (lugarAtualY < 0)
                    {
                        lugarAtualY = 0;
                        bateuParede = true;
                    }
                    if (lugarAtualY > mapa.largura * escala)
                    {
                        lugarAtualY = mapa.largura * escala;
                        bateuParede = true;
                    }
                }
            }
            return distanciaParede;
        }

        public static void desenharScreen()
        {
            int coluna;
            for (coluna = 0; coluna < screenX; coluna++)
            {
                float ang = (player.angRad - (player.angVisao / 2)) + (coluna * (player.angVisao / screenX));
                desenharParede(coluna, distanciaParede(ang));
            }
        }

        public static void desenharParede(int coluna, int distancia)
        {
            int ceiling = (int)((screenY / 2) * ((float)distancia / 450));
            int floor = screenY - ceiling;
            int y;

            char cor;

            if(distancia < 60) cor = '█';
            else if(distancia < 150) cor = '▓';
            else if(distancia < 250) cor = '▒';
            else cor = '░';

            for (y = 0; y < screenY; y++)
            {
                if (coluna < mapa.largura && y < mapa.altura)
                {

                }
                else
                {
                    if (y <= ceiling)
                    {
                        screen[screenX * y + coluna] = ' ';
                    }
                    else if (y > ceiling && y < floor)
                    {
                        screen[screenX * y + coluna] = cor;
                    }
                    else
                    {
                        if(y > screenY - 4) screen[screenX * y + coluna] = 'x';
                        else screen[screenX * y + coluna] = '.';
                    }
                }

            }

        }

        public static void render()
        {
            desenharScreen();
            calculaMiniMapa();
            Console.SetCursorPosition(0, 0);
            Console.Write(screen);
            Console.SetCursorPosition(40, 0);
            Console.Write($"Player Position: ( {player.x} , {player.y} )");
            Console.SetCursorPosition(40, 1);
            Console.Write($"Player Ang: {(-(player.angRad * 180) / 3.14159).ToString("N2")}º");
            Console.SetCursorPosition(40, 2);
            Console.Write($"Distancia parede: {distanciaParede(player.angRad)}");
        }
    }
}
