using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace NetSend.Converters;

/// <summary>
/// Конвертер булева в стиль шрифта. Если true, то вернуть жирный стиль шрифта
/// </summary>
internal class BoolToFontWeightConverter : IValueConverter {
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is bool isFavourite && isFavourite) return FontWeight.Bold;
		return FontWeight.Regular;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}