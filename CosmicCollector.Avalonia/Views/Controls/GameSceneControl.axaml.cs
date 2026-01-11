using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using CosmicCollector.Avalonia.Rendering;

namespace CosmicCollector.Avalonia.Views.Controls;

/// <summary>
/// Контрол отрисовки игрового поля.
/// </summary>
public sealed partial class GameSceneControl : UserControl
{
  /// <summary>
  /// Свойство списка элементов рендера.
  /// </summary>
  public static readonly StyledProperty<IReadOnlyList<RenderItem>?> ItemsProperty =
    AvaloniaProperty.Register<GameSceneControl, IReadOnlyList<RenderItem>?>(nameof(Items));

  private readonly SpriteResolver _spriteResolver = new();
  private readonly List<ContentControl> _controls = new();

  /// <summary>
  /// Инициализирует новый экземпляр <see cref="GameSceneControl"/>.
  /// </summary>
  public GameSceneControl()
  {
    InitializeComponent();
    this.GetObservable(ItemsProperty).Subscribe(new ItemsObserver(this));
  }

  /// <summary>
  /// Элементы рендера.
  /// </summary>
  public IReadOnlyList<RenderItem>? Items
  {
    get => GetValue(ItemsProperty);
    set => SetValue(ItemsProperty, value);
  }

  private void OnItemsChanged(IReadOnlyList<RenderItem>? items)
  {
    UpdateScene();
  }

  private sealed class ItemsObserver : IObserver<IReadOnlyList<RenderItem>?>
  {
    private readonly GameSceneControl _owner;

    public ItemsObserver(GameSceneControl owner)
    {
      _owner = owner;
    }

    public void OnNext(IReadOnlyList<RenderItem>? value)
    {
      _owner.OnItemsChanged(value);
    }

    public void OnError(Exception error)
    {
    }

    public void OnCompleted()
    {
    }
  }

  private void UpdateScene()
  {
    if (SceneCanvas is null)
    {
      return;
    }

    var items = Items;

    if (items is null)
    {
      SceneCanvas.Children.Clear();
      _controls.Clear();
      return;
    }

    EnsureControlCount(items.Count);

    for (var index = 0; index < items.Count; index++)
    {
      var item = items[index];
      var control = _controls[index];

      if (!Equals(control.Tag, item.SpriteKey))
      {
        control.Tag = item.SpriteKey;
        control.Content = _spriteResolver.Resolve(item.SpriteKey);
      }

      control.Width = item.Width;
      control.Height = item.Height;
      Canvas.SetLeft(control, item.X);
      Canvas.SetTop(control, item.Y);
    }
  }

  private void EnsureControlCount(int parCount)
  {
    while (_controls.Count < parCount)
    {
      var holder = new ContentControl
      {
        HorizontalContentAlignment = HorizontalAlignment.Stretch,
        VerticalContentAlignment = VerticalAlignment.Stretch
      };
      _controls.Add(holder);
      SceneCanvas.Children.Add(holder);
    }

    while (_controls.Count > parCount)
    {
      var lastIndex = _controls.Count - 1;
      var holder = _controls[lastIndex];
      SceneCanvas.Children.Remove(holder);
      _controls.RemoveAt(lastIndex);
    }
  }
}
