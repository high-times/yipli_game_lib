public static class ProductMessages
{
    // Mat connection messages
    const string err_mat_connection_android_phone = "Bluetooth Connection lost.\nMake sure that your active Yipli Mat and device bluetooth are turned on.";
    const string err_mat_connection_android_tv = "Mat Connection lost.\nMake sure that your active Yipli Mat is connected to Device via USB cable.";
    const string err_mat_connection_pc = "Mat Connection lost.\nMake sure that your active Yipli Mat is connected to Device via USB cable.";
    const string err_mat_connection_android_phone_register = "Register the YIPLI fitness mat from Yipli Hub to continue playing.";
    const string err_mat_connection_mat_off = "Make sure that your active Yipli mat is turned on.";
    const string err_mat_connection_no_ports = "Required (Serial ports) communication hardware is not available in the system. Mat can't be connected.";

    public static string Err_mat_connection_android_phone => err_mat_connection_android_phone;

    public static string Err_mat_connection_android_tv => err_mat_connection_android_tv;

    public static string Err_mat_connection_pc => err_mat_connection_pc;

    public static string Err_mat_connection_android_phone_register => err_mat_connection_android_phone_register;

    public static string Err_mat_connection_mat_off => err_mat_connection_mat_off;

    public static string Err_mat_connection_no_ports => err_mat_connection_no_ports;
}
