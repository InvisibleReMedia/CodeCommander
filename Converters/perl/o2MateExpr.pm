package o2MateExpr;
use strict;
use warnings;
use Try::Tiny;
use o2MateConst;

sub new {
    my $class = shift;
    my $value = shift;
    my $self = bless { 'value', $value }, $class;
    return $self;
}

sub value { shift->{value} }

use overload
    "+" => "add",
    "-" => "substract",
    "*" => "mult",
    "/" => "div",
    ">" => "sup",
    "<" => "inf",
    "." => "concat";
    
sub add {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $x + $y;
}

sub substract {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $swap ? $y - $x : $x - $y;
}

sub mult {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $x * $y;
}


sub div {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $y != 0 ? $x / $y : 0;
}

sub sup {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $swap ? ($y > $x ? $o2MateConst::true : $o2MateConst::false) : ($x > $y ? $o2MateConst::true : $o2MateConst::false);
}

sub inf {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    for($x, $y) { $_ = 0 if ($_ !~ /^[+-]?\d+$/) }
    $swap ? ($y < $x ? $o2MateConst::true : $o2MateConst::false) : ($x < $y ? $o2MateConst::true : $o2MateConst::false);
}

sub concat {
    my ($self, $other, $swap) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    $x . $y;
}

sub pos {
    my ($self, $other) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    substr($x, $y-1, 1);
}

sub compare {
    my ($self, $other) = @_;
    my ($x, $y) = ($self->{value}, (ref $other eq __PACKAGE__) ? $other->{value} : $other);
    $x eq $y ? $o2MateConst::true : $o2MateConst::false;
}

1;
