package o2MateDict;
use strict;
use warnings;
use XML::Twig;

sub new {
    my $class = shift;
    my(%hashString, %hashArray);
    my $self = bless { 'twig', "" }, $class;
    
    $self->{twig} = new XML::Twig(
                    Twig_handlers => {
                        '/Dictionnaire/value[@type="string"]' => sub { my @par = ($self, @_); string(@par); },
                        '/Dictionnaire/value[@type="array"]' => sub { my @par = ($self, @_); array(@par); }
                    });
    return $self;
}

sub parse {
    my ($self, $fName) = @_;
    $self->{twig}->parsefile($fName);
}

sub string {
    my ($self, $twig, $element) = @_;

    $self->{hashString}{$element->att('name')} = $element->text;

    $twig->purge;
    return 0;
}

sub array {
    my($self, $twig, $element) = @_;
    my($refArray);
    
    if ( !defined($self->{hashArray}{$element->att('name')}) ) {
        my(@array);
        $self->{hashArray}{$element->att('name')} = \@array;
        $refArray = $self->{hashArray}{$element->att('name')};
    } else {
        $refArray = $self->{hashArray}{$element->att('name')};
    }

    foreach my $item ( $element->children('item') ) {
        
        my($nbItems) = scalar(@{$refArray});
        my(%hashFields);
        $refArray->[$nbItems] = \%hashFields;
        my($refFields) = $refArray->[$nbItems];
        
        foreach my $field ( $item->children('field') ) {
            $refFields->{$field->att('name')} = $field->text;
        }
        
    }

    $twig->purge;
    return 0;
}

sub getString() {
    my($self, $name) = @_;
    $self->{hashString}{$name} or "";
}

sub setString() {
    my($self, $name, $value) = @_;
    $self->{hashString}{$name} = $value;
}

sub getCount() {
    my($self, $name) = @_;
    scalar(@{$self->{hashArray}{$name}});
}

sub getField() {
    my($self, $name, $index, $fieldName) = @_;
    $self->{hashArray}{$name}->[$index - 1]{$fieldName} or "";
}

sub setField() {
    my($self, $name, $index, $fieldName, $value) = @_;
    $self->{hashArray}{$name}->[$index - 1]{$fieldName} = $value;
}

1;

