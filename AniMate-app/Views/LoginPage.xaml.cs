namespace AniMate_app.Views;

public partial class LoginPage : ContentPage
{
    private AppTheme currentTheme = App.Current.RequestedTheme;
    public LoginPage()
	{
		InitializeComponent();
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {   
        await Shell.Current.GoToAsync($"profilepage");
    }

    private void usernameEntry_Focused(object sender, FocusEventArgs e)
    {
        loginFrame.BorderColor = Colors.Blue;
    }

    private void usernameEntry_Unfocused(object sender, FocusEventArgs e)
    {
        loginFrame.BorderColor =  currentTheme == AppTheme.Dark ? Colors.Black : Colors.White; ;
    }

    private void passwordEntry_Focused(object sender, FocusEventArgs e)
    {
        passwordFrame.BorderColor = Colors.Blue;        
    }

    private void passwordEntry_Unfocused(object sender, FocusEventArgs e)
    {
        passwordFrame.BorderColor = currentTheme == AppTheme.Dark ? Colors.Black : Colors.White; ;
    }

    private async void registrationLabelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"registrationpage");
        //NavigationPage.SetHasNavigationBar(new RegistrationPage(), true);
        //await Navigation.PushModalAsync(new RegistrationPage());
    }
}