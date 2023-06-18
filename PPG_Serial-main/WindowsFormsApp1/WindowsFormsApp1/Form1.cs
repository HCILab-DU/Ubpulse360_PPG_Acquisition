using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;


namespace WindowsFormsApp1 // 네임스페이스 WindowsFormsAPP1로 정의
{
    // 공개된 여러파일에 나눠 작성되는 'Form1' 클래스 정의
    public partial class Form1 : Form
    {
        // 정적으로 선언된 Serialport 변수에 SerialPort 겍체를 생성하여 할당
        static SerialPort serialPort = new SerialPort();

        public Form1()
        {
            InitializeComponent(); // 폼 초기화
            port_Refresh(); //포트 갱신 및 초기화
            CheckForIllegalCrossThreadCalls = false; //잘못된 스레드 호출 감지
        }

        bool Sync_After = false;
        // 바이트 선언
        byte Packet_TX_Index = 0;
        byte Data_Prev = 0;
        byte PUD0 = 0;
        byte CRD_PUD2_PCDT = 0;
        byte PUD1 = 0;
        byte PacketCount = 0;
        byte PacketCyclicData = 0;
        byte psd_idx = 0;
        static int Ch_Num = 6;
        static int Sample_Num = 1;
        byte[] PacketStreamData = new byte[Ch_Num * 2 * Sample_Num];


        int Parsing_LXSDFT2(byte data_crnt)
        {
            int retv = 0;
            // 만약 Data_Prev 의 값이 0xFF이고 data_crnt 값이 0xFE인경우
            if (Data_Prev == 0xFF && data_crnt == 0xFE)   // Message start point
            {
                // Sync_After를 True로 설정
                Sync_After = true;
                // Packet_TX_Index 를 0으로 설정
                Packet_TX_Index = 0;
            }

            // Data_Prev 값을 data_crnt로 갱신한다.
            Data_Prev = data_crnt;  // Previous data
            // 만약 Sync_After이 True일 경우
            if (Sync_After == true)
            {
                // Packet_TX_Index를 증가시킨다.
                Packet_TX_Index++;  // 0xFE: index 1
                // 만약 Packet_TX_Index 값이 1보다 큰 경우
                if (Packet_TX_Index > 1)    // Do from index 2
                {
                    // Packet_TX_Index가 2인경우 PUD0 변수에 data_crnt값을 저장
                    if (Packet_TX_Index == 2)       // PUD_0
                    {
                        PUD0 = data_crnt;
                    }
                    // 만약 Packet_TX_Index가 3인 경우 CRD_PUD2_PCDT 변수에 data_crnt값을 저장
                    else if (Packet_TX_Index == 3)  // CRD, PUD2, PCDT
                    {
                        CRD_PUD2_PCDT = data_crnt;
                    }
                    // 만약 Packet_TX_Index가 4인 경우 PacketCount 변수에 data_crnt값을 저장
                    else if (Packet_TX_Index == 4)  // PC
                    {
                        PacketCount = data_crnt;
                    }
                    // 만약 Packet_TX_Index가 5인 경우 PUD1 변수에 data_crnt 값을 저장
                    else if (Packet_TX_Index == 5)  // PUD_1
                    {
                        PUD1 = data_crnt;
                    }
                    // 만약 Packet_TX_Index가 6일 경우 PacketCyclicData 값을 data_crnt에 저장
                    else if (Packet_TX_Index == 6)  // PCD
                    {
                        PacketCyclicData = data_crnt;
                    }
                    // 만약 Packet_TX_Index가 6을 초과할 경우 
                    else if (Packet_TX_Index > 6)   // PSDs
                    {
                        psd_idx = (byte)(Packet_TX_Index - 7);  // PSD index
                        PacketStreamData[psd_idx] = data_crnt;
                        // 만약 Packet_TX_Index가 
                        if (Packet_TX_Index == (Ch_Num * 2 * Sample_Num + 6)) // Complete
                        {
                            Sync_After = false;
                            retv = 1;
                        }
                    }
                }
            }
            return retv;
        }

        // SerialPort1_DataReceived 함수 정의
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 시리얼 포트로 가져온 바이트를 정수형인 receivedNumber에 저장
            int receivedNumber = serialPort.BytesToRead;

            // 만약 receivedNumber가 0보다 클 경우
            if (receivedNumber > 0)
            {
                // receivedNumber의 길이로 이루어진 바이트 배열인 buffer를 선언
                byte[] buffer = new byte[receivedNumber];
                // 시리얼 포트로 가져온 데이터를 버퍼라는 배이트배열의 시작점인 0부터 저장한다. receivedNumber라는 데이터로
                serialPort.Read(buffer, 0, receivedNumber);
                // 반복문, foreach 구문을 사용하여 buffer배열에 receivedData를 참조
                foreach (byte receivedData in buffer)
                {
                    // 만약 Parsing_LXSDFT2(receivedData)가 1인 경우
                    if (Parsing_LXSDFT2(receivedData) == 1)
                    {
                        // TextBox에 텍스트를 추가한다. 텍스트 형식은 {0:D2}: {1} ms | 과 같으며 {0:D2}에는 PacketCount가 들어가며 {1}은 ((PUD1 % 0x07) << 8) + PUD0 를 넣는다.
                        textBox_ViewData.AppendText(
                            string.Format("{0:D2}: {1} ms | ",
                                PacketCount,
                                ((PUD1 % 0x07) << 8) + PUD0)
                        );
                        // Ch_Num을 반복하며
                        for (int i = 0; i < Ch_Num; i++)
                        {
                            // TextBox에 텍스트를 추가한다. 텍스트 형식은 {0}이며 ((PacketStreamData[i * 2] & 0x0F) << 8) + PacketStreamData[i * 2 + 1]))를 대입함
                            textBox_ViewData.AppendText(string.Format("{0} ", ((PacketStreamData[i * 2] & 0x0F) << 8) + PacketStreamData[i * 2 + 1]));
                        }
                        textBox_ViewData.AppendText("\r\n");
                        /*if (PacketCount >= 31)
                        {
                            textBox_ViewData.AppendText("\r\n\r\n");
                        }*/
                    }
                }
            }
        }

        private Stopwatch stopwatch = new Stopwatch();
        private void button1_Click(object sender, EventArgs e)
        {

            if (serialPort.IsOpen)
            {
                MessageBox.Show("데이터 저장을 시작합니다.");
                // CSV 파일 경로 및 파일 이름 설정
                string csvFilePath = Path.Combine(Application.StartupPath, "data.csv");
                stopwatch.Start();


                try
                {
                    // CSV 파일을 생성하고 쓰기 위한 StreamWriter 객체 생성
                    using (StreamWriter writer = new StreamWriter(csvFilePath))
                    {
                        int SerialData = serialPort.BytesToRead;

                        writer.WriteLine("Time, Data");

                        if (SerialData > 0)
                        {
                            TimeSpan elapsed = stopwatch.Elapsed;
                            byte[] buffer = new byte[SerialData];
                            serialPort.Read(buffer, 0, SerialData);
                            foreach (byte StreamSaveData in buffer)
                            {
                                if (Parsing_LXSDFT2(StreamSaveData) == 1)
                                {
                                    for (int i = 0; i <Ch_Num; i++)
                                    {
                                        int Streamdata = (((PacketStreamData[i * 2] & 0x0F) << 8) +PacketStreamData[i * 2 + 1]);
                                        string currentTime = elapsed.ToString(@"hh\:mm\:ss\.fff");
                                        string line = string.Format("{0}, {1}", currentTime, Streamdata);
                                        writer.WriteLine(line);
                                    }

                                }

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("저장 오류");
                }

            }
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {

            if (comboBox_Port.Text.Length == 0)
            {
                MessageBox.Show("포트를 선택해주세요.");
                return;
            }

            if (!serialPort.IsOpen)
            {
                serialPort.PortName = comboBox_Port.SelectedItem.ToString();
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialPort.Open();
            }
        }

        private void port_Refresh()
        {
            // 사용이 가능한 시리얼 포트의 이름 목록을 가져옴
            string[] PortNames = SerialPort.GetPortNames();

            // 상자 속의 항목들을 먼저 초기화
            comboBox_Port.Items.Clear();
            // foreach 루프를 사용하여 ComboBox_Port 항목을 추가
            foreach (string portnumber in PortNames)
            {
                comboBox_Port.Items.Add(portnumber);
            }
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            port_Refresh();
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void textBox_ViewData_TextChanged(object sender, EventArgs e)
        {

        }

        private void PPG_Raw_data_print_Click(object sender, EventArgs e)
        {
            Ubpulse_PPG_chart showUbpulse_PPG_Chart = new Ubpulse_PPG_chart();
            showUbpulse_PPG_Chart.ShowDialog();
        }
        /*
        {
            /*
            String strDir = "C:\\Users\\Miran-Laptop\\PPG\\PPG_serial\\PPG_Serial-main"; // CSV 파일 경로와 이름 설정

            using (StreamWriter writer = new StreamWriter(strDir))
            {

            }
            */
        /*
        int StreamNumber = serialPort.BytesToRead;
            string strDir = "C:\\Users\\Miran-Laptop\\PPG\\PPG_serial\\PPG_Serial-main"; // CSV 파일 경로와 이름 설정

            if (serialPort.IsOpen)
            {
                /*
                byte[] buffer = new byte[StreamNumber]; // 시리얼 포트에서 읽어온 데이터를 저장할 버퍼 생성
                serialPort.Read(buffer, 0, StreamNumber); // 시리얼 포트에서 버퍼 크기만큼 데이터 읽기
                */
        /*
                string Streamdata = serialPort1_DataReceived;
                string csvLine = string.Format("{0}", textBox_ViewData);
                /*
                foreach (byte Streamdata in buffer)
                {
                    for (int i = 0; i < Ch_Num; i++)
                    {
                        string Streamdata = String.Format("{0}", (PacketStreamData[i * 2] & 0x0F) << 8) + PacketStreamData[i * 2 + 1];
                    }
                }
                */
//            }

//        }

    }
}
