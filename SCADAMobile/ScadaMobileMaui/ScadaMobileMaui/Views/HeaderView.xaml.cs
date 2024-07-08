namespace ScadaMobileMaui.Views;

public partial class HeaderView : ContentView
{
	public HeaderView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(HeaderView), default(string), propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HeaderView headerView)
        {
            headerView.HeaderTitle.Text = (string)newValue;
        }
    }
}