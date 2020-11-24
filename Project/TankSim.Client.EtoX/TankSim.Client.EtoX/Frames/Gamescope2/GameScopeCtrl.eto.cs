using System;
using Eto.Forms;
using Eto.Drawing;

namespace TankSim.Client.EtoX.Frames.GameScope2
{
	partial class GameScopeCtrl : Panel
	{
		readonly Label lbl_GameTitle = new Label();
		readonly TextBox txt_GameID = new TextBox();
		readonly Label lbl_ConnectionStatus = new Label();
		readonly Button btn_Submit = new Button();

		void InitializeComponent()
		{
			Size = new Size(800, 450);
			lbl_GameTitle.Font = new Font(lbl_GameTitle.Font.Typeface, 26);

			txt_GameID.Font = new Font(txt_GameID.Font.Typeface, 14);
			txt_GameID.Width = 120;
			txt_GameID.KeyDown += Txt_GameID_KeyDown;

			btn_Submit.Text = "Connect";
			btn_Submit.Click += Btn_Submit_Click;



			var layout = new StackLayout
			{
				BackgroundColor = Colors.Gray,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Items =
				{
					lbl_GameTitle,
					new Panel()
					{
						Height = 16
					},
					new Label
					{
						Text = "Game ID"
					},
					txt_GameID,
					lbl_ConnectionStatus,
					btn_Submit
				}
			};

			Content = layout;
			
		}


    }
}
