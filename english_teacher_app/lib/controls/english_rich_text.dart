import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';

//RealRichText

class EnglishRichText extends Text {
  final List<TextSpan> textSpanList;
  final WordOffsetsController controller;
  EnglishRichText(
    this.textSpanList, {
    this.controller,
    Key key,
    TextStyle style,
    TextAlign textAlign = TextAlign.start,
    TextDirection textDirection,
    bool softWrap = true,
    TextOverflow overflow = TextOverflow.clip,
    double textScaleFactor = 1.0,
    int maxLines,
    Locale locale,
  }) : super("",
            style: style,
            textAlign: textAlign,
            textDirection: textDirection,
            softWrap: softWrap,
            overflow: overflow,
            textScaleFactor: textScaleFactor,
            maxLines: maxLines,
            locale: locale);

  @override
  Widget build(BuildContext context) {
    final DefaultTextStyle defaultTextStyle = DefaultTextStyle.of(context);
    TextStyle effectiveTextStyle = style;
    if (style == null || style.inherit)
      effectiveTextStyle = defaultTextStyle.style.merge(style);
    if (MediaQuery.boldTextOverride(context))
      effectiveTextStyle = effectiveTextStyle
          .merge(const TextStyle(fontWeight: FontWeight.bold));

    TextSpan textSpan = TextSpan(
      style: effectiveTextStyle,
      text: "",
      children: textSpanList,
    );

    Widget result = _RichTextWrapper(
        controller: this.controller,
        textAlign: textAlign ?? defaultTextStyle.textAlign ?? TextAlign.start,
        textDirection: textDirection,
        locale: locale,
        softWrap: softWrap ?? defaultTextStyle.softWrap,
        overflow: overflow ?? defaultTextStyle.overflow,
        textScaleFactor:
            textScaleFactor ?? MediaQuery.textScaleFactorOf(context),
        maxLines: maxLines ?? defaultTextStyle.maxLines,
        text: textSpan);
    if (semanticsLabel != null) {
      result = Semantics(
          textDirection: textDirection,
          label: semanticsLabel,
          child: ExcludeSemantics(
            child: result,
          ));
    }
    return result;
  }
}

class _RichTextWrapper extends RichText {
  final WordOffsetsController controller;
  const _RichTextWrapper({
    Key key,
    this.controller,
    @required TextSpan text,
    TextAlign textAlign = TextAlign.start,
    TextDirection textDirection,
    bool softWrap = true,
    TextOverflow overflow = TextOverflow.clip,
    double textScaleFactor = 1.0,
    int maxLines,
    Locale locale,
  })  : assert(text != null),
        assert(textAlign != null),
        assert(softWrap != null),
        assert(overflow != null),
        assert(textScaleFactor != null),
        assert(maxLines == null || maxLines > 0),
        super(
            key: key,
            text: text,
            textAlign: textAlign,
            textDirection: textDirection,
            softWrap: softWrap,
            overflow: overflow,
            textScaleFactor: textScaleFactor,
            maxLines: maxLines,
            locale: locale);

  @override
  RenderParagraph createRenderObject(BuildContext context) {
    assert(textDirection != null || debugCheckHasDirectionality(context));

    return _RealRichRenderParagraph(
      text,
      controller: this.controller,
      textAlign: textAlign,
      textDirection: textDirection ?? Directionality.of(context),
      softWrap: softWrap,
      overflow: overflow,
      textScaleFactor: textScaleFactor,
      maxLines: maxLines,
      locale: locale ?? Localizations.localeOf(context, nullOk: true),
    );
  }
}

class _RealRichRenderParagraph extends RenderParagraph {
  final WordOffsetsController controller;
  _RealRichRenderParagraph(TextSpan text,
      {TextAlign textAlign,
      this.controller,
      TextDirection textDirection,
      bool softWrap,
      TextOverflow overflow,
      double textScaleFactor,
      int maxLines,
      Locale locale})
      : super(
          text,
          textAlign: textAlign,
          textDirection: textDirection,
          softWrap: softWrap,
          overflow: overflow,
          textScaleFactor: textScaleFactor,
          maxLines: maxLines,
          locale: locale,
        );

  @override
  void paint(PaintingContext context, Offset offset) {
    super.paint(context, offset);
    final Canvas canvas = context.canvas;
    final Rect bounds = offset & size;
    canvas.save();
    int textOffset = 0;
    miningOffset( bounds, text, textOffset);
    canvas.restore();
  }

  @override
  void detach() {
    super.detach();
  }

  @override
  void performLayout() {
    super.performLayout();
  }

  int miningOffset(Rect bounds, TextSpan text, int curTextOffset) {
    int textOffset = curTextOffset;

    if (text.children == null) {
      Offset offsetForCaret = getOffsetForCaret(
        TextPosition(offset: textOffset),
        bounds,
      );

      collectOffset(text, offsetForCaret);
      textOffset += text.toPlainText().length;
    } else {
      text.children.forEach((t) {
        textOffset = miningOffset(bounds, text, textOffset);
      });
    }

    return textOffset;
  }

  void collectOffset(TextSpan span, Offset offset) {
    var wspan = span as WordTextSpan;
    if (wspan?.id != null) {
      controller.save(wspan.id, offset.dy);
    }
  }

  void paintImageSpan(PaintingContext context, Offset offset) {
    final Canvas canvas = context.canvas;
    final Rect bounds = offset & size;
    canvas.save();
    int textOffset = 0;
    text.children.forEach((ts) {
      var w = ts as WordTextSpan;
      if (w.children != null) {
        w.children.forEach((t) {
          var w2 = t as WordTextSpan;

          Offset offsetForCaret1 = getOffsetForCaret(
            TextPosition(offset: textOffset),
            bounds,
          );
          if (w2.id != null) {
            controller.save(w2.id, offsetForCaret1.dy);
          }
          textOffset += w2.toPlainText().length;
        });
      } else {
        Offset offsetForCaret2 = getOffsetForCaret(
          TextPosition(offset: textOffset),
          bounds,
        );

        if (w.id != null) {
          controller.save(w.id, offsetForCaret2.dy);
        }
        textOffset += ts.toPlainText().length;
      }
    });
    canvas.restore();
  }
}

class WordTextSpan extends TextSpan {
  final String id;
  WordTextSpan({
    this.id,
    TextStyle style,
    String text,
    List<TextSpan> children,
    GestureRecognizer recognizer,
  }) : super(
            style: style,
            text: text,
            children: children,
            recognizer: recognizer);
}

class WordOffsetsController {
  Map<String, double> offsets = {};
  WordOffsetsController();
  void save(String id, double offset) {
    offsets.addAll({id: offset});
  }

  double getOffset(String id) {
    double rv = 0.0;
    offsets.forEach((k, v) {
      if (k == id) {
        rv = v;
        return;
      }
    });
    return rv;
  }
}
