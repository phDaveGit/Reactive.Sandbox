using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListViewExample
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Client : ReactiveContentPage<ClientViewModel>
	{
		public Client ()
		{
			InitializeComponent ();
		}
	}
}